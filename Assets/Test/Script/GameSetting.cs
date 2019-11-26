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

        // 允許Z是float因為可能有需要場景的前後調整.
        static public Vector3 UnitToWorld(int x,int y, float z)
        {
            Vector3 ret = new Vector3(x, y, 0);
            //只對x,y縮放
            ret = ret * UnitInWorld;
            ret.z = z;
            return ret;
        }

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

        static public Vector3 UnitToWorld(Vector3 input)
        {
            return UnitToWorld((int)input.x,(int)input.y,input.z);
        }

        static public Vector3 WorldToUnit(Vector3 input)
        {
            return WorldToUnit(input.x,input.y,input.z);
        }
    }
}

