using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemContainer : ItemContainerBase
{
    //prefab 塞入
    public UnityEngine.GameObject[] DropPrefabs;


    //隨機道具的index
    private int randomNum;

    public new void Start()
    {
        base.Start();
        Durability = 10.0f;
        //隨機產生道具
        randomNum = Random.Range(0, DropPrefabs.Length);
        Debug.LogFormat("Random type: {0}", randomNum);
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();    
    }
    public override void Broken()
    {
        //產生道具
        //Debug.LogFormat("Container broken! Instantiate obj in {0}", transform.position);
        Instantiate(DropPrefabs[randomNum], transform.position, Quaternion.identity);
        OnDestroy();
        Destroy(this.gameObject);
    }
    public override void Damaged(float damaged)
    {
        Durability -= damaged;
        if (Durability < 1.0)
        {
            //Debug.Log("RandomItemContainer Broken!");
            Broken();
        }
    }
}
