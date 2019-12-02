
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict;
using Photon.Pun;
public class BagList : MonoBehaviourPun
{
    //控制UI
    public List<GameObject> showItem;
    //腳色參考
    GameObject player;
    //塞圖片
    public List<Sprite> icons;
    //道具欄數量
    int Bagsize;
    //拷貝player的道具包
    List<ItemBase> inventory;
    
    void Start()
    {
        Bagsize = 3;
        
        if(player = GameObject.Find("Ruso(Clone)"))
        {
            Debug.Log("UI connect to player");
        }
        foreach (GameObject i in showItem)
        {
            i.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        inventory = player.GetComponent<Character>().GetInventory();
        for (int i=0;i< Bagsize;i++)
        {
            if ( i<inventory.Count)
            {
                showItem[i].SetActive(true);
                int id = inventory[i].id;
                showItem[i].GetComponent<Image>().sprite = icons[inventory[i].id];
                showItem[i].GetComponentInChildren<Text>().text = inventory[i].ItemState().ToString();
              
            }
            else
            {
                showItem[i].SetActive(false);
            }
        }
    }
    public void Useitem0()
    {
        Debug.Log("click item 0");
        player.GetComponent<Character>().UseItem(0);
    }
    public void Useitem1()
    {
        Debug.Log("click item 1");
        player.GetComponent<Character>().UseItem(1);

    }
    public void Useitem2()
    {
        Debug.Log("click item 2");
        player.GetComponent<Character>().UseItem(2);
    }


}
