using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
public class DropItem : ZDObject
{
    //player reference
    private UnityEngine.GameObject player;
    public List<ItemBase> contains = new List<ItemBase>();
   
       
  
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        hpRecover h1 = new hpRecover();
        mpRecover m1 = new mpRecover();
        contains.Add(h1);
        contains.Add(m1);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }
    public void AddToPlayer(Character _player)
    {
        Debug.Log("DropItems: add to player");
        foreach (ItemBase i in contains)
        {
            Debug.Log("play pick dropitem");
           // Debug.Log("Player now have {0} items",_player.inv);
            _player.GetItem(i);
        }
        
        Destroy(this.gameObject);        
    }
}
