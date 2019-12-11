using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZoneDepict.Rule
{
    public enum TypeDepth
    {

        MapObject = 0,
        Effect,
        DroppedItem,
        ItemContainer,
        RemoteCharacter,
        LocalCharacter,    
        Map,
    }
    public enum AttackType
    {
        N, A, B, R, Cancel
    }
    public enum RoomPlayerState
    {
        Enter , Casting, Ready
    }

    public class ZDGameRule : MonoBehaviour
    {
        public static class CustomPropsKey
        {
            public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";
        }
        // Const Game Variable declare
        public const uint MAP_WIDTH_UNIT = 21;
        public const uint MAP_HEIGHT_UNIT = 17;
        public const int MAX_PLAYERS = 4;
        public const float UnitInWorld = 2.5f;
        public const float MAP_WIDTH_WORLD = MAP_WIDTH_UNIT * UnitInWorld;
        public const float MAP_HEIGHT_WORLD = MAP_HEIGHT_UNIT * UnitInWorld;
        public const int TOUCH_TAP_BOUND_FRAMES = 20;

        // transform the input position from zonedepict unit to world unit.(z axis is remained the same)
        static public Vector3 UnitToWorld(int x, int y, float z)
        {
            Vector3 ret = new Vector3(x, y, 0);
            //只對x,y縮放
            ret  *= UnitInWorld;
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
                if (pos < UnitInWorld / 2)
                {
                    ret[i] = 0;
                }
                else
                {
                    pos -= UnitInWorld / 2;
                    ret[i] = (neg ? -1 : 1) * (int)(pos / UnitInWorld + 1);
                }
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
            if(Mathf.Abs(input.x) >Mathf.Abs(input.y))
            {
                return new Vector2(input.x < 0 ? -1 : 1 , 0);
            }
            else
            {
                return new Vector2(0, input.y < 0 ? -1 : 1);
            }
            //float degree = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            //degree = (degree + 45) % 360;
            //degree = (degree < 0 ? 3 - (int)Mathf.Abs(degree) / 90 : (int)Mathf.Abs(degree) / 90);
            //switch ((int)degree)
            //{
            //    case 0:
            //        return new Vector2(1, 0);
            //    case 1:
            //        return new Vector2(0, 1);
            //    case 2:
            //        return new Vector2(-1, 0);
            //    case 3:
            //        return new Vector2(0, -1);
            //    default:
            //        return new Vector2(0, 0);
            //}
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
            return 90 * (degree < 0 ? 3 - (int)Mathf.Abs(degree) / 90 : (int)Mathf.Abs(degree) / 90); ;
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


        // Given QuadDirection and Angle , return new offset
        static public Vector2 RotateVector2(Vector2 input,float degree)
        {
            int x = Mathf.RoundToInt(Mathf.Cos(degree) * input.x - Mathf.Sin(degree) * input.y);
            int y = Mathf.RoundToInt(Mathf.Sin(degree) * input.x + Mathf.Cos(degree) * input.y);
            return new Vector2(x, y);
        }
        //static public float CalculateDistance(Vector2 ThisPos,Vector2 Target)
        //{
        //    return Mathf.Sqrt(Mathf.Pow(ThisPos.x - Target.x, 2) + Mathf.Pow(ThisPos.y - Target.y, 2));
        //}
        //static public float CalculateDistance(Vector2 ThisPos)
        //{
        //    return CalculateDistance(ThisPos, new Vector2(0, 0));
        //}
        static public AttackType DirectionToType(Vector2 Direction)
        {
            // Customer to set where should be N,A,B or R
            AttackType Type;
            switch (QuadAngle(Direction))
            {
                case 0:
                    Type = AttackType.B;
                    break;
                case 90:
                    Type = AttackType.R;
                    break;
                case 180:
                    Type = AttackType.A;
                    break;
                case 270:
                    Type = AttackType.Cancel;
                    break;
                default:
                    Type = AttackType.Cancel;
                    break;
            }
            return Type;
        }
    }
}

