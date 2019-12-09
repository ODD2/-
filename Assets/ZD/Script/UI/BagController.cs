using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
namespace ZoneDepict.UI
{
    public class BagController : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {

            hover = true;
        }

        #region BagUI
        int choosingID; //正在選哪個道具
        bool hover;     //選擇中
        public Sprite defaultSprite;
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
        #endregion
        public GameObject debugmess;
        public List<GameObject> useEffect;
        // Start is called before the first frame update
        void Start()
        {
            choosingID = -1;
            hideall();
            Bagsize = 3;
            hover = false;
            if (player = ZDGameManager.PlayerObject)
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
            FreshInventory();

            if (hover)
            {
                debugmess.GetComponent<Text>().text = "hover";
            }
            else
            {
                debugmess.GetComponent<Text>().text = "no hover";
            }
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Stationary)
                {
                    if (hover)
                    {
                        showall();
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (hover)
                    {
                        if (choosingID!=-1)
                        {
                            useItem(choosingID);
                            choosingID = -1;
                        }

                        hover = false;
               
                    }
                    hideall();
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (hover)
                    {
                        bool hasselect = false;
                        for (int i = 0; i < showItem.Count; i++)
                        {
                            if (showItem[i].GetComponent<UIstate>().ishover)
                            {
                                //  imessage.GetComponent<Text>().text = "item" + i.ToString();
                                choosingID = i;
                                hasselect = true;
                                break;
                            }
                        }
                        if (!hasselect)
                        {
                            choosingID = -1;
                        }
                    }
                }
            }

        }
        void FreshInventory()
        {
            inventory = player.GetComponent<Character>().GetInventory();
            for (int i = 0; i < Bagsize; i++)
            {
                if (i < inventory.Count)
                {
                    showItem[i].GetComponent<Image>().sprite = icons[inventory[i].id];
                    showItem[i].GetComponentInChildren<Text>().text = inventory[i].ItemState().ToString();
                }
                else
                {
                    showItem[i].GetComponentInChildren<Text>().text = "";
                    showItem[i].GetComponent<Image>().sprite = defaultSprite;
                }
            }
        }
        void hideall()
        {
            foreach (GameObject i in showItem)
            {
                i.SetActive(false);
            }
        }
        void showall()
        {
            foreach (GameObject i in showItem)
            {
                i.SetActive(true);
            }
        }
        void useItem(int i)
        {
            
            player.GetComponent<Character>().UseItem(i);


        }
    }
}