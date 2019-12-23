using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZoneDepict.Rule
{
    #region enum Feild
    public enum EAttackType
    {
        N, A, B, R, Skill
    }

    public enum EActorType
    {
        CoverMapObject,
        Effect,
        LocalCharacter,
        RemoteCharacter,
        GeneralMapObject,
        ItemContainer,
        DroppedItem,
        Total,
    }

    public enum EObjectType
    {
        //Body
        Obstacle,
        Transient,
        //Functional
        Shelter,
        //Characteristic
        Character,
        ACollect,
        ADamage,
        Total,
    }

    public enum RoomPlayerState
    {
        Enter , Casting, Ready
    }

    public enum ZDTeams
    {
        T0,
        T1,
        Total,
    }

    public enum CharacterState
    {
        Alive,
        Dead,
    }

    public enum GameActorLayers
    {
        MapObjectMin = 1,
        MapObjectMax = -1,
        RestrictZone = -2,
        CharacterInfo = -3,
        MainCamera = -5,
    }
    #endregion

    public static class CustomPropsKey
    {
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
    }

    public static  class ZDGameRule 
    {
        // Const Game Variable declare
        public const uint MAP_WIDTH_UNIT = 21;
        public const uint MAP_HEIGHT_UNIT = 17;
        public const int MAX_PLAYERS = 4;
        public const float UNIT_IN_WORLD = 2.5f;
        public const float WORLD_IN_PIXEL = 100.0f;
        public const float UNIT_IN_PIXEL = UNIT_IN_WORLD * 100.0f;
        public const float MAP_WIDTH_WORLD = MAP_WIDTH_UNIT * UNIT_IN_WORLD;
        public const float MAP_HEIGHT_WORLD = MAP_HEIGHT_UNIT * UNIT_IN_WORLD;
        public const int TOUCH_TAP_BOUND_FRAMES = 20;
        public const float SINGLE_ACTOR_DEPTH = 0.00003051757f;
        //public const float SINGLE_ACTOR_DEPTH = 1e-5f;
        public const float UNIT_DEPTH = SINGLE_ACTOR_DEPTH * (int)EActorType.Total;

        //Audio
        public const float MAX_AUDIO_DISTANCE = 10 * UNIT_IN_WORLD;

        public static class CrossTrack
        {
            public const float NextTrackDelay = 3.0f;
            public const float TrackDurationConst = 10.0f;
            public const int TrackCountsConst = 2;
            public const float MissionFailedPunish = 5.0f;
        }

        public static class RestrictZone
        {
            public const float HurtThresh = 1.0f;
        }

        // transform the input position from zonedepict unit to world unit.(z axis is remained the same)
        static public Vector3 UnitToWorld(int x, int y, float z)
        {
            Vector3 ret = new Vector3(x, y, 0);
            //只對x,y縮放
            ret *= UNIT_IN_WORLD;
            ret.z = z;
            return ret;
        }
        static public Vector3 UnitToWorld(Vector3 input)
        {
            return UnitToWorld((int)input.x, (int)input.y, input.z);
        }



        //transform the input position from world unit to zonedepict unit.(z axis is remained the same)
        static public Vector3 WorldToUnit(float x, float y, float z)
        {
            Vector3 ret = new Vector3(x, y, z);
            //只對XY做縮放
            for (int i = 0; i < 2; i++)
            {
                float pos = ret[i];
                bool neg = pos < 0;
                pos = Mathf.Abs(pos);
                if (pos < UNIT_IN_WORLD / 2)
                {
                    ret[i] = 0;
                }
                else
                {
                    pos -= UNIT_IN_WORLD / 2;
                    ret[i] = (neg ? -1 : 1) * (int)(pos / UNIT_IN_WORLD + 1);
                }
                ret[i] = Mathf.Round(ret[i]);
            }

            return ret;
        }
        static public Vector3 WorldToUnit(Vector3 input)
        {
            return WorldToUnit(input.x, input.y, input.z);
        }
        static public Vector2 WorldToUnit(Vector2 input)
        {
            Vector3 Result = WorldToUnit(input.x, input.y, 0);
            return (Vector2)Result;
        }

        //transform the input position into a scale of zonedepict unit in the world.(z axis is remained the same)
        static public Vector3 WorldUnify(float x, float y, float z)
        {
            return UnitToWorld(WorldToUnit(x, y, z));
        }
        static public Vector3 WorldUnify(Vector3 input)
        {
            return UnitToWorld(WorldToUnit(input));
        }

        //Strictly transform the input direction  into one of the four orientations.
        static public Vector2 QuadrifyDirection2Unit(Vector2 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return new Vector2(input.x < 0 ? -1 : 1, 0);
            }
            else
            {
                return new Vector2(0, input.y < 0 ? -1 : 1);
            }
        }

        //Constraint the input vector to the four directions.
        static public Vector2 QuadrifyDirection(Vector2 input)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                return new Vector2(input.x, 0);
            }
            else
            {
                return new Vector2(0, input.y);
            }
        }

        //Constraint the input vector to the four directions according to the given pivot.
        static public Vector2 QuadrifyDirection(Vector2 input, Vector2 pivot)
        {
            return pivot + QuadrifyDirection(input - pivot);
        }

        static public float QuadAngle(float input)
        {
            float degree = (input + 45) % 360;
            return Mathf.Round(90 * (degree < 0 ? 3 - (int)Mathf.Abs(degree) / 90 : (int)Mathf.Abs(degree) / 90));
        }

        static public float QuadAngle(Vector2 input)
        {
            return QuadAngle(Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg);
        }

        static public float QuadRadius(float input)
        {
            return QuadAngle(input * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        }
        static public float QuadRadius(Vector2 input)
        {
            return QuadRadius(Mathf.Atan2(input.y, input.x));
        }

        static public float ActorDepth(EActorType eActorType)
        {
            return (int)(eActorType + 1) * SINGLE_ACTOR_DEPTH;
        }
        static public float WorldDepth(float y)
        {
            return Mathf.Round(y / UNIT_IN_WORLD) * UNIT_DEPTH;
        }
        static public float WorldActorDepth(float y, EActorType eActorType)
        {
            return WorldDepth(y) + ActorDepth(eActorType);
        }
        
        // Given QuadDirection and Angle , return new offset
        static public Vector2 RotateVector2(Vector2 input,float degree)
        {
            int x = Mathf.RoundToInt(Mathf.Cos(degree) * input.x - Mathf.Sin(degree) * input.y);
            int y = Mathf.RoundToInt(Mathf.Sin(degree) * input.x + Mathf.Cos(degree) * input.y);
            return new Vector2(x, y);
        }
    }
}

