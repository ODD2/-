using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZoneDepict
{
    public class ZDAssetTable
    {
        public class DefaultAssetObject
        {
            public string path;
            GameObject sample;

            public GameObject Get()
            {
                if (sample == null)
                {
                    sample = Resources.Load<GameObject>(path);
                    if (sample == null)
                    {
                        Debug.Log("Error Cannot Load From " + path);
                    }
                }
                return sample;
            }
        }

        private static readonly Dictionary<string, DefaultAssetObject> SampleObject = new Dictionary<string, DefaultAssetObject>
        {
            //Map Object
            {
                "Grass",
                new DefaultAssetObject
                {
                    path = "MapObjects/Grass",
                }
            },
            {
                "Bamboo",
                new DefaultAssetObject
                {
                    path = "MapObjects/Bamboo",
                }
            },
            {
                "Tree",
                new DefaultAssetObject
                {
                    path = "MapObjects/Tree",
                }
            },
            {
                "Stone",
                new DefaultAssetObject
                {
                    path = "MapObjects/Stone",
                }
            },
            {
                "Mountain",
                new DefaultAssetObject
                {
                    path = "MapObjects/Mountain",
                }
            },
            {
                "WoodBox",
                new DefaultAssetObject
                {
                    path = "MapObjects/WoodBox",
                }
            },
            {
                "SpawnPoint",
                new DefaultAssetObject
                {
                    path = "MapObjects/SpawnPoint",
                }
            },
            //Character
            //Item Effect
            {
                "AttackBoostEffect",
                new DefaultAssetObject
                {
                    path = "Effects/AttackBoost",
                }
            },
            {
                "HpRecoveredEffect",
                new DefaultAssetObject
                {
                    path = "Effects/HpRecovered",
                }
            },
            {
                "MpRecoveredEffect",
                new DefaultAssetObject
                {
                    path = "Effects/MpRecovered",
                }
            },
            //DropItem
            {
                "MpDrop",
                new DefaultAssetObject
                {
                    path = "DropItems/MpDrop",
                }
            },
            {
                "HpDrop",
                new DefaultAssetObject
                {
                    path = "DropItems/HpDrop",
                }
            },
            {
                "BoostAtkDrop",
                new DefaultAssetObject
                {
                    path = "DropItems/BoostAtkDrop",
                }
            },
            //Others
            {
                "AudioPlayer",
                new DefaultAssetObject
                {
                    path =  "AudioPlayer/AudioPlayer",
                }
            },
        };

        static public GameObject GetObject(string name)
        {
            if (SampleObject.ContainsKey(name))
            {
                return SampleObject[name].Get();
            }
            return null;
        }

        static public string GetPath(string name)
        {
            if (SampleObject.ContainsKey(name))
            {
                return SampleObject[name].path;
            }
            return "";
        }
    }
}

