﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
namespace ZoneDepict
{
    public enum ETypeZDO
    {
        Obstacle,
        Transient,
        ACollect,
        ADamage,
        Total,
    }

    public class ZDGridBlock
    {
        private List<ZDObject>[] CategorizeList = new List<ZDObject>[(int)ETypeZDO.Total];
        private Dictionary<ZDObject, List<ETypeZDO>> Passengers = new Dictionary<ZDObject, List<ETypeZDO>>(); 
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
                    List<ZDObject> TargetList = CategorizeList[(int)Type];
                    if (TargetList == null) return false;
                    Result &= TargetList.Remove(TargetObj);
                    if (TargetList.Count == 0) TargetList = null;
                }
                Result &= Passengers.Remove(TargetObj);
                return Result;
            }
            return false;
            
        }

        public void Add(ZDObjectRecord TargetRecord)
        {
            Passengers.Add(TargetRecord.Owner, TargetRecord.Types);
            foreach(var Type in TargetRecord.Types)
            {
                ref List<ZDObject> TargetList = ref CategorizeList[(int)Type];
                if (TargetList == null) TargetList = new List<ZDObject>();
                TargetList.Add(TargetRecord.Owner);
            }
        }

        public List<ZDObject> GetTypeList(ETypeZDO Type)
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
        public List<ETypeZDO> Types;
        public ZDObject Owner;
    }

    public class ZDMap
    {
        //Create a static 2d array to record all the ZoneDepict Objects in this game.
        static private ZDGridBlock[,] RecordGrid = new ZDGridBlock[ZDGameRule.MAP_WIDTH_UNIT, ZDGameRule.MAP_HEIGHT_UNIT];
        static private Dictionary<ZDObject, ZDObjectRecord> Recorder = new Dictionary<ZDObject, ZDObjectRecord>();
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
            
            Recorder[Caller] = new ZDObjectRecord
            {
                Location = MapLoc,
                Types = AuditObjTypes(Caller),
                Owner = Caller,
            };
            TargetBlock.Add(Recorder[Caller]);
            return;

        }
        
        static public void UnRegister(ZDObject Caller)
        {
            //Chec if caller is registered
            if (IsRegistered(Caller))
            {
                (uint, uint) MapLoc = Recorder[Caller].Location;
                ZDGridBlock TargetBlock  = RecordGrid[MapLoc.Item1, MapLoc.Item2];
                if (TargetBlock.Remove(Caller))
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

        static public void UpdateLocation(ZDObject Caller)
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
                    Recorder[Caller].Location = NewMapLoc;
                    NewBlock.Add(Recorder[Caller]);
                }
                return;
            }
        }

        static public bool IsRegistered(ZDObject Caller)
        {
            return Recorder.ContainsKey(Caller);
        }

        static List<ETypeZDO> AuditObjTypes(ZDObject Caller)
        {
            List<ETypeZDO> TrueTypes = new List<ETypeZDO>();
            foreach(var AuditType in Caller.Types)
            {
                switch (AuditType)
                {
                    case ETypeZDO.ACollect:
                        if(Caller is IACollectObject) TrueTypes.Add(ETypeZDO.ACollect);
                    break;
                    case ETypeZDO.ADamage:
                        if (Caller is IADamageObject) TrueTypes.Add(ETypeZDO.ADamage);
                    break;
                    default:
                        TrueTypes.Add(AuditType);
                    break;
                }
            }
            return TrueTypes;
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
            (uint, uint) MapLoc = Recorder[Caller].Location;
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
        static public List<ZDObject> HitAt(int x, int y, ZDObject Caller,ETypeZDO Type)
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
        static public List<ZDObject> HitAt(Vector2 input, ZDObject Caller,ETypeZDO Type)
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
