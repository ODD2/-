using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            // To fix ArrowScale
            
            for (int i = 1; i < 6; i++)
            {
                ArrowScale[i] = ((ZDGameRule.UnitInWorld / 3) * i);
            }
            MoveIndicator.transform.localScale = new Vector3(ZDGameRule.UnitInWorld, ZDGameRule.UnitInWorld, 1);
        }

        // Update is called once per frame
        void Update()
        {
            
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
    }
}

