using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneDepict
{
    public class ZDMap
    {
        //Create a static 2d array to record all the ZoneDepict Objects in this game.
        static private List<ZDObject>[,] RecordGrid = new List<ZDObject>[GameSetting.MAP_WIDTH, GameSetting.MAP_HEIGHT];

        static public void Register(ZDObject Caller)
        {
            // -- Preparation --
            var UnitLoc = GameSetting.WorldToUnit(Caller.transform.position);
            //Force caller to adjust its position.
            Caller.transform.position = GameSetting.UnitToWorld(UnitLoc);

            // -- Register start -- 
            // Check player's unit location is not outside the map.
            if (IsUnitInMap(UnitLoc))
            {
                //check if caller already registered.
                if (Caller.Registered)
                {
                    //if registed update its location in map.
                    UpdateLocation(Caller);
                    return;
                }
                else
                {
                    //if caller is not registered
                    Vector2 MapLoc = UnitToMap(UnitLoc);

                    //add caller into new location
                    AddCallerToMap(MapLoc, Caller);
                    //record map info for caller.
                    Caller.Registered = true;
                    Caller.ValidInMap = true;
                    Caller.MapLocX = (int)MapLoc.x;
                    Caller.MapLocY = (int)MapLoc.y;
                    return;
                }
            }
            else
            {
                Caller.Registered = false;
                Caller.ValidInMap = false;
            }
        }

        static public void UnRegister(ZDObject Caller)
        {
            if (!Caller.Registered) return;
            else if (!IsUnitInMap(Caller.MapLocX, Caller.MapLocY)) return;
            else if (RecordGrid[Caller.MapLocX, Caller.MapLocY] == null) return;
            else if (RecordGrid[Caller.MapLocX, Caller.MapLocY].Exists(Obj => Obj.Equals(Caller)))
            {
                RemoveCallerFromMap(Caller);
                Caller.MapLocX = -1;
                Caller.MapLocY = -1;
                Caller.ValidInMap = false;
                Caller.Registered = false;
            }
        }

        static public void UpdateLocation(ZDObject Caller)
        {
            var UnitLoc = GameSetting.WorldToUnit(Caller.transform.position);
            if(IsUnitInMap(UnitLoc))
            {
                if(RecordGrid[Caller.MapLocX,Caller.MapLocY] == null)
                {
                    Caller.ValidInMap = false;
                    return;
                }
                else if (RecordGrid[Caller.MapLocX, Caller.MapLocY].Exists(Obj => Obj.Equals(Caller)))
                {
                    Vector2 MapLoc = UnitToMap(UnitLoc);
                    
                    if(MapLoc.x == Caller.MapLocX && MapLoc.y == Caller.MapLocY)
                    {
                        //location remains, no need to update.
                        Caller.ValidInMap = true;
                        return;
                    }

                    RemoveCallerFromMap(Caller);
                    AddCallerToMap(MapLoc, Caller);

                    //record map info for caller.
                    Caller.MapLocX = (int)MapLoc.x;
                    Caller.MapLocY = (int)MapLoc.y;
                    Caller.ValidInMap = true;
                    return;
                }
            }
            else
            {
                Caller.ValidInMap = false;
            }
        }

        static private void  RemoveCallerFromMap(ZDObject Caller)
        {
            //remove caller from old location.
            RecordGrid[Caller.MapLocX, Caller.MapLocY].Remove(Caller);
            //clean up empty lists.
            if (RecordGrid[Caller.MapLocX, Caller.MapLocY].Count == 0)
            {
                RecordGrid[Caller.MapLocX, Caller.MapLocY] = null;
            }
        }

        static private void AddCallerToMap(int x, int y, ZDObject Caller)
        {
            if (RecordGrid[(int)x, (int)y] == null)
            {
                //Create list if map has nothing at this location.
                RecordGrid[(int)x, (int)y] = new List<ZDObject>();
            }
            //add caller into new location
            RecordGrid[(int)x, (int)y].Add(Caller);
        }
        static private void AddCallerToMap(Vector2 MapLoc, ZDObject Caller)
        {
            AddCallerToMap((int)MapLoc.x, (int)MapLoc.y,Caller);
        }


        static public Vector2 UnitToMap(int x,int y)
        {
            return new Vector2(x + GameSetting.MAP_WIDTH / 2,
                                            y + GameSetting.MAP_HEIGHT / 2);
        }
        static public Vector2  UnitToMap(Vector2 UnitLoc)
        {
            return UnitToMap((int)UnitLoc.x, (int)UnitLoc.y);
        }

        static public bool IsUnitInMap(int x , int y)
        {
            return !(Mathf.Abs(x) > (int)(GameSetting.MAP_WIDTH / 2) ||
                        Mathf.Abs(y) > (int)(GameSetting.MAP_HEIGHT / 2));
        }
        static public bool IsUnitInMap(Vector2 UnitLoc)
        {
            return IsUnitInMap((int)UnitLoc.x,(int) UnitLoc.y);
        }

        static public List<ZDObject> HitAt(int x,int y,ZDObject Caller)
        {
            x += Caller.MapLocX;
            y += Caller.MapLocY;
            Debug.Log("ZDMap - HitAt: " + x + ", " + y);
            if(x < 0 || y < 0 || !(x<GameSetting.MAP_WIDTH && y < GameSetting.MAP_HEIGHT))
            {
                return null;
            }
            return RecordGrid[x, y];
        }
        static public List<ZDObject> HitAt(Vector2 input, ZDObject Caller)
        {
            return  HitAt((int)input.x,(int)input.y,Caller);
        }
    }
}
