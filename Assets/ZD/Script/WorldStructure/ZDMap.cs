using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
namespace ZoneDepict
{
    public class ZDMap
    {
        //Create a static 2d array to record all the ZoneDepict Objects in this game.
        static private List<ZDObject>[,] RecordGrid = new List<ZDObject>[ZDGameRule.MAP_WIDTH_UNIT, ZDGameRule.MAP_HEIGHT_UNIT];
        static private Dictionary<ZDObject, (uint, uint)> RecordLocation = new Dictionary<ZDObject, (uint, uint)>();

        static public void Register(ZDObject Caller)
        {
            // -- Preparation --
            //Force caller to adjust its position on the map.
            RevisePosition(Caller);

            // -- Register start -- 
            // Check if caller already registered.
            if (IsRegistered(Caller))
            {
                //if registed update its location in map.
                UpdateLocation(Caller);
                return;
            }
            //if caller is not registered
            (uint, uint) MapLoc = WorldToMap(Caller.transform.position);
            //add caller into new location
            AddCallerToMap(MapLoc, Caller);
            return;

        }
        
        static public void UnRegister(ZDObject Caller)
        {
            //Chec if caller is registered
            if (IsRegistered(Caller))
            {
                (uint, uint) MapLoc = RecordLocation[Caller];
                List<ZDObject> TargetList = RecordGrid[MapLoc.Item1, MapLoc.Item2];
                if (TargetList == null)
                {
                    //TODO: Log Error -> This must not happen in general.
                    return;
                }
                else if (TargetList.Exists(Obj => Obj.Equals(Caller)))
                {
                    RemoveCallerFromMap(Caller);
                    //clear object record.
                    RecordLocation.Remove(Caller);
                }
            }
        }

        static public void UpdateLocation(ZDObject Caller)
        {
            //Update location only if caller is inside the map and it's registered.
            if(IsWorldInMap(Caller.transform.position) && IsRegistered(Caller))
            {
                (uint, uint) NewMapLoc = WorldToMap(Caller.transform.position);
                (uint, uint) PrevMapLoc = RecordLocation[Caller];
                List<ZDObject> TargetList = RecordGrid[PrevMapLoc.Item1, PrevMapLoc.Item2];
                if (TargetList == null || !TargetList.Exists(Obj => Obj.Equals(Caller)))
                {
                    //TODO: Error Log: This should not happen in general.
                    return;
                }

                if (NewMapLoc == PrevMapLoc)
                {
                    //location remains, no need to update.
                    return;
                }

                //Update Position.
                RemoveCallerFromMap(Caller);
                AddCallerToMap(NewMapLoc, Caller);
                return;
            }
        }

        static private void  RemoveCallerFromMap(ZDObject Caller)
        {
            if (RecordLocation.ContainsKey(Caller))
            {
                (uint, uint) MapLoc = RecordLocation[Caller];
                //remove caller from old location.
                RecordGrid[MapLoc.Item1, MapLoc.Item2].Remove(Caller);
                //clean up empty lists.
                if (RecordGrid[MapLoc.Item1, MapLoc.Item2].Count == 0)
                {
                    RecordGrid[MapLoc.Item1, MapLoc.Item2] = null;
                }
            }

        }

        static private void AddCallerToMap(uint x, uint y, ZDObject Caller)
        {
            if (RecordGrid[x, y] == null)
            {
                //Create list if map has nothing at this location.
                RecordGrid[x, y] = new List<ZDObject>();
            }
            //add caller into new location
            RecordGrid[x, y].Add(Caller);
            RecordLocation[Caller] = (x, y);
        }
        static private void AddCallerToMap((uint, uint) MapLoc, ZDObject Caller)
        {
            AddCallerToMap(MapLoc.Item1, MapLoc.Item2, Caller);
        }
        static private void AddCallerToMap(Vector2 MapLoc, ZDObject Caller)
        {
            AddCallerToMap((uint)MapLoc.x, (uint)MapLoc.y,Caller);
        }

        static public bool IsRegistered(ZDObject Caller)
        {
            return RecordLocation.ContainsKey(Caller);
        }

        static public (uint,uint) UnitToMap(int x,int y)
        {
            uint _x = (uint)(x + ZDGameRule.MAP_WIDTH_UNIT / 2);
            uint _y = (uint)(y + ZDGameRule.MAP_HEIGHT_UNIT / 2);

            //Constraint the position if it's out of the map's scope
            if (_x >= ZDGameRule.MAP_WIDTH_UNIT) _x = ZDGameRule.MAP_WIDTH_UNIT;
            else if (_x < 0) _x = 0;

            if (_y >= ZDGameRule.MAP_HEIGHT_UNIT) _y = ZDGameRule.MAP_HEIGHT_UNIT;
            else if (_y < 0) _y = 0;

            return (_x, _y);
        }
        static public (uint,uint)  UnitToMap(Vector2 UnitLoc)
        {
            return UnitToMap((int)UnitLoc.x, (int)UnitLoc.y);
        }

        static public (uint,uint) WorldToMap(float x, float y)
        {
            return UnitToMap(ZDGameRule.WorldToUnit(x, y, 0));
        }
        static public (uint,uint) WorldToMap(Vector2 WorldLoc)
        {
            return WorldToMap(WorldLoc.x, WorldLoc.y);
        }


        static public bool IsUnitInMap(int x , int y)
        {
            return !(Mathf.Abs(x) > (int)(ZDGameRule.MAP_WIDTH_UNIT / 2) ||
                        Mathf.Abs(y) > (int)(ZDGameRule.MAP_HEIGHT_UNIT / 2));
        }
        static public bool IsUnitInMap(Vector2 UnitLoc)
        {
            return IsUnitInMap((int)UnitLoc.x,(int) UnitLoc.y);
        }

        static public bool IsWorldInMap(float x, float y)
        {
            return !(Mathf.Abs(x) > (ZDGameRule.MAP_WIDTH_WORLD / 2) ||
                        Mathf.Abs(y) > (ZDGameRule.MAP_HEIGHT_WORLD / 2));
        }
        static public bool IsWorldInMap(Vector2 WorldLoc)
        {
            return IsWorldInMap(WorldLoc.x, WorldLoc.y);
        }

        static public void RevisePosition(ZDObject Caller)
        {
            Vector3 UnitLoc = ZDGameRule.WorldToUnit(Caller.transform.position);
            if(Mathf.Abs(UnitLoc.x) > ZDGameRule.MAP_WIDTH_UNIT / 2)
            {
                UnitLoc.x = (UnitLoc.x > 0 ? 1 : -1) * ZDGameRule.MAP_WIDTH_UNIT / 2;
            }
            if (Mathf.Abs(UnitLoc.y) > ZDGameRule.MAP_HEIGHT_UNIT / 2)
            {
                UnitLoc.y = (UnitLoc.y > 0 ? 1 : -1) * ZDGameRule.MAP_HEIGHT_UNIT / 2;
            }
            Caller.transform.position = ZDGameRule.UnitToWorld(UnitLoc);
        }

        static public List<ZDObject> HitAt(int x,int y,ZDObject Caller)
        {
            //Debug.Log("Offset : " + x + " , " + y);
            (uint, uint) MapLoc = RecordLocation[Caller];
            x += (int)MapLoc.Item1;
            y += (int)MapLoc.Item2;
//            Debug.Log("ZDMap - HitAt: " + x + ", " + y);
            if(x < 0 || y < 0 || !(x< ZDGameRule.MAP_WIDTH_UNIT && y < ZDGameRule.MAP_HEIGHT_UNIT))
            {
                return null;
            }
            return RecordGrid[x, y];
        }
        static public List<ZDObject> HitAt(Vector2 input, ZDObject Caller)
        {
            return  HitAt((int)input.x,(int)input.y,Caller);
        }

        static public List<ZDObject> HitAtUnit(int x, int y)
        {
            (uint, uint) MapLoc = UnitToMap(x, y);
            //Debug.Log("ZDMap - HitAt: " + MapLoc.Item1 + ", " + MapLoc.Item2);
            if (MapLoc.Item1 < 0 || MapLoc.Item2 < 0 ||
                !(MapLoc.Item1 < ZDGameRule.MAP_WIDTH_UNIT && MapLoc.Item2 < ZDGameRule.MAP_HEIGHT_UNIT))
            {
                return null;
            }
            return RecordGrid[MapLoc.Item1, MapLoc.Item2];
        }
        static public List<ZDObject> HitAtUnit(Vector2 UnitLoc)
        {
            return HitAtUnit((int)UnitLoc.x, (int)UnitLoc.y);
        }
    }
}
