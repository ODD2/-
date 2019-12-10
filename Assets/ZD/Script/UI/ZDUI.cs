using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict.Rule;
namespace ZoneDepict.UI
{
    public class ZDUI : MonoBehaviour
    {
        public RectTransform HealthBar;
        public RectTransform HealthBarBG;
        public RectTransform MagicBar;
        public RectTransform MagicBarBG;

        public Sprite[] AttackSources;
        public GameObject AttackIndicator;
        public GameObject MoveIndicator;
        private Transform Attack;
        private Transform Move;
        private float FrameFix = 0.01f;
        private float[] ArrowScale = { 0, 0, 0, 0, 0, 0 };

        // Start is called before the first frame update
        void Start()
        {
            MoveIndicator = Instantiate(MoveIndicator);
            AttackIndicator = Instantiate(AttackIndicator);
            MoveIndicator.SetActive(false);
            AttackIndicator.SetActive(false);
            Attack = AttackIndicator.GetComponent<Transform>();
            Move = MoveIndicator.GetComponent<Transform>();

            for (int i = 0; i < 6; i++)
            {
                ArrowScale[i] = ((ZDGameRule.UnitInWorld / 3) * i);
            }
            MoveIndicator.transform.localScale = new Vector3(ZDGameRule.UnitInWorld, ZDGameRule.UnitInWorld, 1);
        }

        public void SetAttackIndicator(Vector2 Position)
        {
            // World Position
            Attack.position = Position;
            AttackIndicator.SetActive(true);
        }

        public void CancelAttackIndicator()
        {
            AttackIndicator.SetActive(false);
        }

        public void UpdateAttackCircle(AttackType Type)
        {
            AttackIndicator.GetComponent<SpriteRenderer>().sprite = AttackSources[(int)Type];
            
        }

        public void SetMoveIndicator(Vector2 Pos, float Degree, float Scale)
        {
            Move.rotation = Quaternion.Euler(0, 0, Degree - 90); // Fix Assets's 90 degree
            Move.position = Pos;
            if ((int)Scale > 5) Scale = 5;
            float DoScale = ArrowScale[(int)Scale]; // Fix 
            Move.localScale = new Vector3(ZDGameRule.UnitInWorld, DoScale, 0);
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

        public void UpdateHPBar(float maxHP,float HP)
        {
            HealthBar.sizeDelta = new Vector2((maxHP / HealthBarBG.rect.width) * HP, HealthBar.rect.height);
        }
      
        public void UpdateMPBar(float maxMP, float MP)
        {
            MagicBar.sizeDelta = new Vector2((maxMP / MagicBarBG.rect.width) * MP, MagicBar.rect.height);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                HealthBar.sizeDelta += new Vector2(-1, 0);
            }
            
        }
    }
}

