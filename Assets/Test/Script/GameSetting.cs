using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneDepict
{
    public class GameSetting
    {
        public const uint MAP_WIDTH = 15;
        public const uint MAP_HEIGHT = 11;
        public const float UnitInWorld = 2;

        // transform th input position from zonedepict unit to world unit.(z axis is remained the same)
        static public Vector3 UnitToWorld(int x,int y, float z)
        {
            Vector3 ret = new Vector3(x, y, 0);
            //只對x,y縮放
            ret = ret * UnitInWorld;
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
                if(pos < UnitInWorld / 2)
                {
                    ret[i] = 0;
                }
                else
                {
                    pos -= UnitInWorld / 2;
                    ret[i] = (neg ? -1 : 1 )* (int)(pos / UnitInWorld+1);
                }
            }

            return ret;
        }
        static public Vector3 WorldToUnit(Vector3 input)
        {
            return WorldToUnit(input.x,input.y,input.z);
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
        static public Vector2 QuadDirection(Vector2 input)
        {
            float degree = Mathf.Atan2(input.y,input.x) * Mathf.Rad2Deg;
            degree = (degree + 45) % 360;
            degree = (degree < 0 ? 3 - (int)Mathf.Abs(degree) / 90 : (int)Mathf.Abs(degree) / 90 );
            switch ((int)degree)
            {
                case 0:
                    return new Vector2(1, 0);
                case 1:
                    return new Vector2(0, 1);
                case 2:
                    return new Vector2(-1, 0);
                case 3:
                    return new Vector2(0, -1);
                default:
                    return new Vector2(0, 0);
            }
        }
    }
}

