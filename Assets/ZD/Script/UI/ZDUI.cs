using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict.Rule;
namespace ZoneDepict.UI
{
    public class ZDUI : MonoBehaviour
    {
        public GameObject AttackIndicator;
        public GameObject MoveIndicator;
        private Transform Attack;
        private Transform Move;
        private float FrameFix = 0.008f;
        #region BagUI
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
        private float[] ArrowScale = { 0,0, 0, 0, 0, 0 };
        // Start is called before the first frame update
        void Start()
        {
            MoveIndicator = Instantiate(MoveIndicator);
            AttackIndicator = Instantiate(AttackIndicator);
            MoveIndicator.SetActive(false);
            AttackIndicator.SetActive(false);
            Attack = AttackIndicator.GetComponent<Transform>();
            Move = MoveIndicator.GetComponent<Transform>();
            //Bag initial
            Bagsize = 3;
            if (player = ZDGameManager.PlayerObject)
            {
                Debug.Log("UI connect to player");
            }
            foreach (GameObject i in showItem)
            {
                i.SetActive(false);
            }
            // To fix ArrowScale
            
            for (int i = 0; i < 6; i++)
            {
                ArrowScale[i] = ((ZDGameRule.UnitInWorld / 3) * i);
            }
            MoveIndicator.transform.localScale = new Vector3(ZDGameRule.UnitInWorld, ZDGameRule.UnitInWorld, 1);
        }

        // Update is called once per frame
        void Update()
        {
            inventory = player.GetComponent<Character>().GetInventory();
            for (int i = 0; i < Bagsize; i++)
            {
                if (i < inventory.Count)
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

        public void SetAttackIndicator(Vector2 Position)
        {
            Attack.position = Position;
            AttackIndicator.SetActive(true);
        }

        public void CancelAttackIndicator()
        {
            AttackIndicator.SetActive(false);
        }

        public void SetMoveIndicator(Vector2 Pos, float Degree, float Scale)
        {
            Move.rotation = Quaternion.Euler(0, 0, Degree - 90); // Fix Assets's 90 degree
            Move.position = Pos;
            float DoScale = ArrowScale[(int)Scale ]; // Fix 
            Move.localScale = new Vector3(ZDGameRule.UnitInWorld, DoScale,0);
            MoveIndicator.SetActive(true);

        }

        public void CancelMoveIndicator()
        {
            MoveIndicator.SetActive(false);

        }

        public void SetAttackOpacity(int Frame)
        {
            AttackIndicator.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, Frame * FrameFix);
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
}

