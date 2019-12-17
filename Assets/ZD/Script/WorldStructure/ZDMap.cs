using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;

namespace ZoneDepict.Map
{
    public class ZDGridBlock
    {
        private List<ZDObject>[] CategorizeList = new List<ZDObject>[(int)EObjectType.Total];
        private Dictionary<ZDObject, HashSet<EObjectType>> Passengers = new Dictionary<ZDObject, HashSet<EObjectType>>(); 
        public bool IsEmpty()
        {
            foreach (var list in CategorizeList)
            {
                if(list != null && list.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool Exists(ZDObject TargetObj)
        {
            return Passengers.ContainsKey(TargetObj);
        }
        
        public bool Remove(ZDObject TargetObj)
        {
            if (Passengers.ContainsKey(TargetObj))
            {
                bool Result = true;
                foreach (var Type in Passengers[TargetObj])
                {
                    ref List<ZDObject> TargetList = ref CategorizeList[(int)Type];
                    if (TargetList == null) return false;
                    Result &= TargetList.Remove(TargetObj);
                    if (TargetList.Count == 0) TargetList = null;
                }
                Result &= Passengers.Remove(TargetObj);
                return Result;
            }
            return false;
            
        }

        public void Add(ZDObject  TargetObj, HashSet<EObjectType> Types)
        {
            Passengers.Add(TargetObj, Types);
            foreach(var Type in Types)
            {
                ref List<ZDObject> TargetList = ref CategorizeList[(int)Type];
                if (TargetList == null) TargetList = new List<ZDObject>();
                TargetList.Add(TargetObj);
            }
        }

        public List<ZDObject> GetTypeList(EObjectType Type)
        {
            return CategorizeList[(int)Type];
        }

        public List<ZDObject> GetAll()
        {
            List<ZDObject> AllObjects = new List<ZDObject>();
            foreach (var lists in CategorizeList)
            {
                if(lists!=null)AllObjects.AddRange(lists);
            }

            if (AllObjects.Count == 0) return null;
            else return AllObjects;
        }
    }

    public class ZDObjectRecord
    {
        public (uint, uint) Location;
        public List<EObjectType> Types;
        public ZDObject Owner;
    }

    public static class ZDMap
    {
        //Create a static 2d array to record all the ZoneDepict Objects in this game.
        static private ZDGridBlock[,] RecordGrid;
        static public Dictionary<ZDObject, ZDObjectRecord> Recorder;

        static ZDMap()
        {
            InitializeMap();
        }

        static void InitializeMap()
        {
            RecordGrid  = new ZDGridBlock[ZDGameRule.MAP_WIDTH_UNIT, ZDGameRule.MAP_HEIGHT_UNIT];
            Recorder = new Dictionary<ZDObject, ZDObjectRecord>();

            //Initilize Record Grid
            for (int i = 0; i < ZDGameRule.MAP_WIDTH_UNIT; ++i)
            {
                for (int j = 0; j < ZDGameRule.MAP_HEIGHT_UNIT; ++j)
                {
                    RecordGrid[i, j] = new ZDGridBlock();
                }
            }
        }

        static public void ResetMap()
        {
            InitializeMap();
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

            //Create Caller Record.
            Recorder[Caller] = new ZDObjectRecord
            {
                Location = MapLoc,
                Owner = Caller,
            };
            AddToRecordGrids(Recorder[Caller]);
            return;

        }
        
        static public void UnRegister(ZDObject Caller)
        {
            //Chec if caller is registered
            if (IsRegistered(Caller))
            {
                if (RemoveFromRecordGrids(Recorder[Caller]))
                {
                    //clear object record.
                    Recorder.Remove(Caller);
                }
                else
                {
                    //TODO: Log Error -> This must not happen in general.
                    return;
                }
            }
        }

        static public bool UpdateLocation(ZDObject Caller)
        {
            //Update location only if caller is inside the map and it's registered.
            if(IsWorldInMap(Caller.transform.position) && IsRegistered(Caller))
            {
                (uint, uint) NewMapLoc = WorldToMap(Caller.transform.position);
                (uint, uint) PrevMapLoc = Recorder[Caller].Location;
                ZDGridBlock PrevBlock= RecordGrid[PrevMapLoc.Item1, PrevMapLoc.Item2];
                ZDGridBlock NewBlock = RecordGrid[NewMapLoc.Item1, NewMapLoc.Item2];
                if (NewMapLoc == PrevMapLoc)
                {
                    //location remains, no need to update.
                    return false;
                }
                else if (RemoveFromRecordGrids(Recorder[Caller]))
                {
                    //TODO: Error Log: This should not happen in general.
                    return false;
                }
                else
                {
                    //Update Position.
                    Recorder[Caller].Location = NewMapLoc;
                    AddToRecordGrids(Recorder[Caller]);
                }
                return true;
            }
            return false;
        }

        static public bool IsRegistered(ZDObject Caller)
        {
            return Recorder.ContainsKey(Caller);
        }

        static void AddToRecordGrids(ZDObjectRecord CallerRecord)
        {
            (uint, uint) Origin = CallerRecord.Location;
            ZDObject Caller = CallerRecord.Owner;
            (int, int) OffsetLoc;
            foreach (var region in Caller.Regions)
            {
                OffsetLoc = ((int)Origin.Item1 + region.Key.x, (int)Origin.Item2 + region.Key.y);
                if (IsValidMapLoc(OffsetLoc.Item1, OffsetLoc.Item2))
                {
                    //add caller into new location
                    RecordGrid[OffsetLoc.Item1, OffsetLoc.Item2].Add(Caller, region.Value);
                }
            }
        }
        static bool RemoveFromRecordGrids(ZDObjectRecord CallerRecord)
        {
            bool Result = true;
            (uint, uint) Origin = CallerRecord.Location;
            ZDObject Caller = CallerRecord.Owner;
            (int, int) OffsetLoc;
            foreach (var region in Caller.Regions)
            {
                OffsetLoc = ((int)Origin.Item1 + region.Key.x, (int)Origin.Item2 + region.Key.y);
                if (IsValidMapLoc(OffsetLoc.Item1, OffsetLoc.Item2))
                {
                    //add caller into new location
                    Result &= RecordGrid[OffsetLoc.Item1, OffsetLoc.Item2].Remove(Caller);
                }
            }
            return Result;
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

        static public bool IsValidMapLoc(int x,int y)
        {
            return (x >= 0) && (x < ZDGameRule.MAP_WIDTH_UNIT) && (y >= 0) && (y < ZDGameRule.MAP_HEIGHT_UNIT);
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

        static public (uint,uint) GetObjectLocation(ZDObject Target)
        {
            if (IsRegistered(Target))
            {
                return Recorder[Target].Location;
            }
            else
            {
                return (0, 0);
            }
        }

        static public List<ZDObject> HitAt(int x,int y,ZDObject Caller)
        {
            (uint, uint) MapLoc = Recorder[Caller].Location;
            x += (int)MapLoc.Item1;
            y += (int)MapLoc.Item2;
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
        static public List<ZDObject> HitAt(int x, int y, ZDObject Caller,EObjectType Type)
        {
            (uint, uint) MapLoc = Recorder[Caller].Location;
            x += (int)MapLoc.Item1;
            y += (int)MapLoc.Item2;
            if (x < 0 || y < 0 || !(x < ZDGameRule.MAP_WIDTH_UNIT && y < ZDGameRule.MAP_HEIGHT_UNIT))
            {
                return null;
            }
            return RecordGrid[x, y].GetTypeList(Type);
        }
        static public List<ZDObject> HitAt(Vector2 input, ZDObject Caller,EObjectType Type)
        {
            return HitAt((int)input.x, (int)input.y, Caller, Type);
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
        static public List<ZDObject> HitAtUnit(int x, int y, EObjectType Type)
        {
            (uint, uint) MapLoc = UnitToMap(x, y);
            //Debug.Log("ZDMap - HitAt: " + MapLoc.Item1 + ", " + MapLoc.Item2);
            if (MapLoc.Item1 < 0 || MapLoc.Item2 < 0 ||
                !(MapLoc.Item1 < ZDGameRule.MAP_WIDTH_UNIT && MapLoc.Item2 < ZDGameRule.MAP_HEIGHT_UNIT))
            {
                return null;
            }
            return RecordGrid[MapLoc.Item1, MapLoc.Item2].GetTypeList(Type);
        }
        static public List<ZDObject> HitAtUnit(Vector2 UnitLoc, EObjectType Type)
        {
            return HitAtUnit((int)UnitLoc.x, (int)UnitLoc.y,Type);
        }

        static public List<ZDObject> HitAtObject(ZDObject Target,EObjectType Type)
        {
            if (Recorder.ContainsKey(Target))
            {
                List<ZDObject> Result = new List<ZDObject>();
                ZDObjectRecord TargetRecord = Recorder[Target];
                (uint, uint) Origin = TargetRecord.Location;
                ZDObject Caller = TargetRecord.Owner;
                (int, int) OffsetLoc;
                foreach (var region in Caller.Regions)
                {
                    OffsetLoc = ((int)Origin.Item1 + region.Key.x, (int)Origin.Item2 + region.Key.y);
                    if (IsValidMapLoc(OffsetLoc.Item1, OffsetLoc.Item2))
                    {
                        List<ZDObject> GetList = RecordGrid[OffsetLoc.Item1, OffsetLoc.Item2].GetTypeList(Type);
                        if (GetList != null) Result.AddRange(GetList);
                    }
                }
                if (Result.Count != 0) return Result;
                else return null;
            }
            return null;
        }
    }
}
