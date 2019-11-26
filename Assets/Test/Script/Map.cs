using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneDepict
{
    public class ZDMap
    {
        //建立記錄地圖物件的2D陣列
        static private List<ZDObject>[,] RecordGrid = new List<ZDObject>[GameSetting.MAP_HEIGHT, GameSetting.MAP_WIDTH];

        static public bool Register(Vector2 UnitLoc, ZDObject Caller)
        {
            if (UnitInMap(UnitLoc))
            {
                if (Caller.Registered)
                {
                    return UpdateLocation(UnitLoc, Caller);
                }
                else
                {
                    Vector2 MapLoc = UnitToMap(UnitLoc);
                    //如果這個地方還沒有List
                    if (RecordGrid[(int)MapLoc.x, (int)MapLoc.y]==null)
                    {
                        RecordGrid[(int)MapLoc.x, (int)MapLoc.y] = new List<ZDObject>();
                    }
                    //加入List
                    RecordGrid[(int)MapLoc.x, (int)MapLoc.y].Add(Caller);
                    //紀錄
                    Caller.Registered = true;
                    Caller.MapLocX = (int)MapLoc.x;
                    Caller.MapLocY = (int)MapLoc.y;
                    return true;
                }
            }
            return false;
        }
        //輸入的X,Y是以原點為0,0，單位是方格個數

        static public bool UpdateLocation(Vector2 UnitLoc, ZDObject Caller)
        {
            if(UnitInMap(UnitLoc))
            {
                if(RecordGrid[Caller.MapLocX,Caller.MapLocY] == null)
                {
                    return false;
                }
                else if (RecordGrid[Caller.MapLocX, Caller.MapLocY].Exists(Obj => Obj.Equals(Caller)))
                {
                    Vector2 MapLoc = UnitToMap(UnitLoc);
                    RecordGrid[Caller.MapLocX, Caller.MapLocY].Remove(Caller);

                    if(RecordGrid[(int)MapLoc.x, (int)MapLoc.y] == null)
                    {
                        //如果位置還沒有List
                        RecordGrid[(int)MapLoc.x, (int)MapLoc.y] = new List<ZDObject>();
                    }

                    RecordGrid[(int)MapLoc.x, (int)MapLoc.y].Add(Caller);
                    Caller.MapLocX = (int)MapLoc.x;
                    Caller.MapLocY = (int)MapLoc.y;
                    return true;
                }
            }
            return false;
        }

        static public Vector2  UnitToMap(Vector2 UnitLoc)
        {
            return new Vector2(UnitLoc.x + GameSetting.MAP_HEIGHT / 2 ,
                                            UnitLoc.y + GameSetting.MAP_WIDTH / 2);
        }

        static public bool UnitInMap(Vector2 UnitLoc)
        {
            return !(Mathf.Abs(UnitLoc.x) > (int)(GameSetting.MAP_HEIGHT / 2) ||
                        Mathf.Abs(UnitLoc.y) > (int)(GameSetting.MAP_WIDTH / 2));
        }
    }
}
