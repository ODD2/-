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
            }
        };

        static public GameObject GetObject(string name)
        {
            if (SampleObject.ContainsKey(name))
            {
                return SampleObject[name].Get();
            }
            return null;
        }
    }
}

