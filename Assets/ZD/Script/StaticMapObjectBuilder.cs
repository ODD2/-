using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ZoneDepict;
using ZoneDepict.Rule;
[Serializable]
public struct SpawnObjectConfig
{
    public string name;
    public Vector2[] pos;
    public bool line;
    public bool flipX;
    public bool flipY;
}

public class StaticMapObjectBuilder : MonoBehaviour
{
    static private SpawnObjectConfig[] StaticMapObjectConfigs = {
        new SpawnObjectConfig
        {
            name = "Grass",
            pos = new Vector2[]
            {
                new Vector2(-1,1),
                new Vector2(-2,2),
                new Vector2(-1,-1),
                new Vector2(-2,-2),

                new Vector2(-5,-1),
                new Vector2(-6,-2),
                new Vector2(-2,-5),
                new Vector2(-3,-5),

                new Vector2(-9,2),
                new Vector2(-9,-4),
                new Vector2(-10,2),
                new Vector2(-10,1),
                new Vector2(-10,-3),
                new Vector2(-10,-5),

                new Vector2(-1,8),
                new Vector2(-2,8),
                new Vector2(0,-8),
                new Vector2(-1,-8)
            },
            flipX = true,
            flipY = true,
            line = true,
        },
        new SpawnObjectConfig
        {
            name = "Bamboo",
            pos = new Vector2[]
            {
                new Vector2(-9,-6),
                new Vector2(-8,-6),
                new Vector2(-5,-5),
                new Vector2(-4,-5),
                new Vector2(-4,7),
                new Vector2(-3,7),
                new Vector2(3,-8),
                new Vector2(4,-8),
                new Vector2(4,4),
                new Vector2(5,4),
                new Vector2(8,5),
                new Vector2(9,5),
            },
        },
        new SpawnObjectConfig
        {
            name = "Tree",
            pos = new Vector2[]
            {
                new Vector2(-10,-8),
                new Vector2(-5,5),
                new Vector2(4,6),
                new Vector2(9,7)
            },
        },
        new SpawnObjectConfig
        {
            name = "Stone",
            pos = new Vector2[]
            {
                new Vector2(-10,0),
                new Vector2(-10,-2),
                new Vector2(-6,-6),
                new Vector2(-6,-7),
                new Vector2(-5,8),
                new Vector2(-5,7),
                new Vector2(-1,4),
                new Vector2(-1,5),
            },
            line  = true,
            flipX = true,
            flipY = true
        },
        new SpawnObjectConfig
        {
            name = "Mountain",
            pos = new Vector2[]
            {
                new Vector2(9,-4)
            },
            flipX = true,
            flipY = true
        },
    };
    // Start is called before the first frame update
    void Start()
    {
        Quaternion ZeroRotation = new Quaternion() {
            eulerAngles = new Vector3(0, 0, 0)
        };
        foreach (var config in StaticMapObjectConfigs)
        {
            if (config.line)
            {
                int end = config.pos.Length;
                if (config.pos.Length % 2 != 0) end -= 1;
                //Iterate In 2 Units
                for(int i = 0; i < end; i+=2)
                {
                    int x, _x,dx;
                    int y, _y,dy;
                    x = (int)config.pos[i].x; _x = (int)config.pos[i + 1].x; dx = _x > x ? 1:-1;
                    y = (int)config.pos[i].y; _y = (int)config.pos[i + 1].y; dy = _y > y ? 1:-1;
                    for (int tx = x;; tx +=dx )
                    {
                        for (int ty = y;; ty += dy)
                        {
                            InstAtUnit(config, new Vector3(tx, ty, 0));
                            if (ty == _y) break;
                        }
                        if (tx == _x) break;
                    }
                }
            }
            else
            {
                foreach (var pos in config.pos)
                {
                    InstAtUnit(config, pos);
                }
            }
        }

        Destroy(this.gameObject);
    }

    void InstAtUnit(SpawnObjectConfig target,Vector3 Pos)
    {
        GameObject obj = ZDAssetTable.GetObject(target.name);
        Pos *= ZDGameRule.UnitInWorld;
        Instantiate(obj, Pos , new Quaternion());
        if(target.flipX || target.flipY)
        {
            if (target.flipX) Pos.x = -Pos.x;
            if (target.flipY) Pos.y = -Pos.y;
            Instantiate(obj, Pos, new Quaternion());
        }
    }
}
