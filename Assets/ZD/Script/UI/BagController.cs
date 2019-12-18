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
            FrameBlock = true;
        }

        private bool FrameBlock;
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
        // Start is called before the first frame update
        void Start()
        {
            choosingID = -1;
            hideall();
            Bagsize = 3;
            hover = false;
            player = ZDGameManager.GetPlayerProps().Object;
            if (player != null)
            {
                Debug.Log("UI connect to player");
            }
        }

        // Update is called once per frame

        void Update()
        {
            if (hover)
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Stationary)
                    {

                        FreshInventory();
                        showall();

                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {


                        if (choosingID != -1)
                        {
                            useItem(choosingID);
                            choosingID = -1;
                        }

                        hover = false;
                        hideall();


                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {

                        FreshInventory();
                        bool hasselect = false;
                        for (int i = 0; i < showItem.Count; i++)
                        {
                            if (showItem[i].GetComponent<UIstate>().ishover)
                            {
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
            if (player)
            {
                inventory = player.GetComponent<Character>().GetInventory();
                for (int i = 0; i < Bagsize; i++)
                {
                    if (i < inventory.Count)
                    {
                        showItem[i].GetComponent<Image>().sprite = icons[inventory[i].id];
                        showItem[i].GetComponentInChildren<Text>().text = inventory[i].ItemState();
                    }
                    else
                    {
                        showItem[i].GetComponentInChildren<Text>().text = "";
                        showItem[i].GetComponent<Image>().sprite = defaultSprite;
                    }
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

        public bool GetHover()
        {
            return hover;
        }

        public void SetHover(bool hoverSet)
        {
            hover = hoverSet;
        }

        public bool GetFrameBlock()
        {
            return FrameBlock;
        }

        public void SetBlockFrame(bool FrameBlockSet)
        {
            FrameBlock = FrameBlockSet;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // hover = true;

        }
    }
}
