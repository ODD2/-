﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict.Rule;

namespace ZoneDepict.UI
{
    public class ZDUI : MonoBehaviour
    {
        static public ZDUI Instance;

        [Header("The Indicator img of Attacking")]
        public GameObject AttackIndicator;
        public Sprite[] AttackSources;
        
        
        [Header("The Indicator img of Moving")]
        public GameObject MoveIndicator;
        
        private Transform Attack;
        private Transform Move;
        private float FrameFix = 0.01f;
        private float[] ArrowScale = { 0, 0, 0, 0, 0, 0 };
        
        // Start is called before the first frame update
        void Start()
        {
            if (Instance != null) Destroy(gameObject);
            else Instance = this;

            MoveIndicator = Instantiate(MoveIndicator);
            MoveIndicator.transform.position += new Vector3(0, 0, -4);
            AttackIndicator = Instantiate(AttackIndicator);
            MoveIndicator.SetActive(false);
            AttackIndicator.SetActive(false);
            Attack = AttackIndicator.GetComponent<Transform>();
            Move = MoveIndicator.GetComponent<Transform>();

            for (int i = 0; i < 6; i++)
            {
                ArrowScale[i] = ((ZDGameRule.UNIT_IN_WORLD / 3) * i);
            }
            MoveIndicator.transform.localScale = new Vector3(ZDGameRule.UNIT_IN_WORLD, ZDGameRule.UNIT_IN_WORLD, 1);
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

        public void UpdateAttackCircle(EAttackType Type)
        {
            AttackIndicator.GetComponent<SpriteRenderer>().sprite = AttackSources[(int)Type];
            
        }

        public void SetMoveIndicator(Vector2 Pos, float Degree, float Scale)
        {
            Vector3 ZPos = new Vector3(Pos.x, Pos.y, -4);
            Move.rotation = Quaternion.Euler(0, 0, Degree - 90); // Fix Assets's 90 degree
            Move.position = ZPos;
            if ((int)Scale > 5) Scale = 5;
            float DoScale = ArrowScale[(int)Scale]; // Fix 
            Move.localScale = new Vector3(ZDGameRule.UNIT_IN_WORLD, DoScale, 0);
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

        private void Update()
        {
            
            
        }

        private void OnDestroy()
        {
	        if(Instance == this){
            	Instance = null;
	        }
        }

    }
}

