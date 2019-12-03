using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
namespace ZoneDepict
{
    public enum ETypeZDO
    {
        Obstacle,
        Transient,
        Total,
    }

    public class ZDGridBlock
    {
        private List<ZDObject>[] Record = new List<ZDObject>[(int)ETypeZDO.Total];
        private ZDObject test;

        public ZDGridBlock()
        {
        }

        public bool IsEmpty()
        {
            foreach (var list in Record)
            {
                if(list != null && list.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Exists(ZDObject TargetObject)
        {
            var TypeList = GetListForObject(TargetObject);
            if (TypeList != null && TypeList.Exists(Obj => Obj.Equals(TargetObject)))
            {
                return true;
            }
            return false;
        }
    
        private ref List<ZDObject> GetListForObject(ZDObject TypeObject)
        {
            if (TypeObject is ZDObstacle) return  ref Record[(int)ETypeZDO.Obstacle];
            else return ref Record[(int)ETypeZDO.Transient];
        }
        
        public bool Remove(ZDObject TargetObj)
        {
            ref  List<ZDObject> TargetList = ref  GetListForObject(TargetObj);
            if (TargetList == null) return false;
            bool Result =   TargetList.Remove(TargetObj);
            if (TargetList.Count == 0) TargetList = null;
            return Result;
        }

        public void Add(ZDObject TargetObj)
        {
            ref List<ZDObject> TargetList = ref GetListForObject(TargetObj);
            if (TargetList == null) TargetList = new List<ZDObject>();
            TargetList.Add(TargetObj);
        }

        public List<ZDObject> GetTypeList(ETypeZDO Type)
        {
            return Record[(int)Type];
        }

        public List<ZDObject> GetAll()
        {
            List<ZDObject> AllObjects = new List<ZDObject>();
            foreach (var lists in Record)
            {
                if(lists!=null)AllObjects.AddRange(lists);
            }

            if (AllObjects.Count == 0) return null;
            else return AllObjects;
        }
    }

    public class ZDMap
    {
        //Create a static 2d array to record all the ZoneDepict Objects in this game.
        static private ZDGridBlock[,] RecordGrid = new ZDGridBlock[ZDGameRule.MAP_WIDTH_UNIT, ZDGameRule.MAP_HEIGHT_UNIT];
        static private Dictionary<ZDObject, (uint, uint)> RecordLocation = new Dictionary<ZDObject, (uint, uint)>();
        static ZDMap()
        {
            //Initilize Record Grid
            for(int i = 0; i < ZDGameRule.MAP_WIDTH_UNIT; ++i)
            {
                for (int j = 0; j < ZDGameRule.MAP_HEIGHT_UNIT; ++j)
                {
                    RecordGrid[i, j] = new ZDGridBlock();
                }
            }
        }
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
            ZDGridBlock TargetBlock = RecordGrid[MapLoc.Item1, MapLoc.Item2];
            //add caller into new location
            TargetBlock.Add(Caller);
            RecordLocation[Caller] = MapLoc;
            return;

        }
        
        static public void UnRegister(ZDObject Caller)
        {
            //Chec if caller is registered
            if (IsRegistered(Caller))
            {
                (uint, uint) MapLoc = RecordLocation[Caller];
                ZDGridBlock TargetBlock  = RecordGrid[MapLoc.Item1, MapLoc.Item2];
                if (TargetBlock.Remove(Caller))
                {
                    //clear object record.
                    RecordLocation.Remove(Caller);
                }
                else
                {
                    //TODO: Log Error -> This must not happen in general.
                    return;
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
                ZDGridBlock PrevBlock= RecordGrid[PrevMapLoc.Item1, PrevMapLoc.Item2];
                ZDGridBlock NewBlock = RecordGrid[NewMapLoc.Item1, NewMapLoc.Item2];
                if (NewMapLoc == PrevMapLoc)
                {
                    //location remains, no need to update.
                    return;
                }
                else if (!PrevBlock.Remove(Caller))
                {
                    //TODO: Error Log: This should not happen in general.
                    return;
                }
                else
                {
                    //Update Position.
                    NewBlock.Add(Caller);
                    RecordLocation[Caller] = NewMapLoc;
                }
                return;
            }
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
            return RecordGrid[x, y].GetAll();
        }
        static public List<ZDObject> HitAt(Vector2 input, ZDObject Caller)
        {
            return  HitAt((int)input.x,(int)input.y,Caller);
        }

        static public List<ZDObject> HitAtUnit(int x, int y)
        {
            (uint, uint) MapLoc = UnitToMap(x, y);
            Debug.Log("ZDMap - HitAt: " + MapLoc.Item1 + ", " + MapLoc.Item2);
            if (MapLoc.Item1 < 0 || MapLoc.Item2 < 0 ||
                !(MapLoc.Item1 < ZDGameRule.MAP_WIDTH_UNIT && MapLoc.Item2 < ZDGameRule.MAP_HEIGHT_UNIT))
            {
                return null;
            }
            return RecordGrid[MapLoc.Item1, MapLoc.Item2].GetAll();
        }
        static public List<ZDObject> HitAtUnit(Vector2 UnitLoc)
        {
            return HitAtUnit((int)UnitLoc.x, (int)UnitLoc.y);
        }
        static public List<ZDObject> HitAtUnit(int x, int y, ETypeZDO Type)
        {
            (uint, uint) MapLoc = UnitToMap(x, y);
            Debug.Log("ZDMap - HitAt: " + MapLoc.Item1 + ", " + MapLoc.Item2);
            if (MapLoc.Item1 < 0 || MapLoc.Item2 < 0 ||
                !(MapLoc.Item1 < ZDGameRule.MAP_WIDTH_UNIT && MapLoc.Item2 < ZDGameRule.MAP_HEIGHT_UNIT))
            {
                return null;
            }
            return RecordGrid[MapLoc.Item1, MapLoc.Item2].GetTypeList(Type);
        }
        static public List<ZDObject> HitAtUnit(Vector2 UnitLoc, ETypeZDO Type)
        {
            return HitAtUnit((int)UnitLoc.x, (int)UnitLoc.y,Type);
        }
    }
}
