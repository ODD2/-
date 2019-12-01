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
        private float FrameFix = 0.003f;
        // Start is called before the first frame update
        void Start()
        {
            MoveIndicator = Instantiate(MoveIndicator);
            AttackIndicator = Instantiate(AttackIndicator);
            MoveIndicator.SetActive(false);
            AttackIndicator.SetActive(false);
            Attack = AttackIndicator.GetComponent<Transform>();
            Move = MoveIndicator.GetComponent<Transform>();
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
            int DoScale = (int)(Scale / ZDGameRule.UnitInWorld)+1;
            Move.localScale = new Vector3(DoScale,DoScale,0);
            MoveIndicator.SetActive(true);

        }
        public void CancelMoveIndicator()
        {
            MoveIndicator.SetActive(false);
            
        }
        public void SetAttackOpacity(int Frame)
        {
            if (1 - (Frame * FrameFix) <= 0)
            {
                CancelAttackIndicator();
                
            }
            AttackIndicator.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (1-(Frame * FrameFix)) > 0 ? (1 - (Frame * FrameFix)) : 0);
            
        }
    }
}

