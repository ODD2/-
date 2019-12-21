using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict.Rule;

namespace ZoneDepict.UI
{
    public class ZDUI : MonoBehaviour
    {
        static public ZDUI Instance;
        
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
            MoveIndicator.SetActive(false);
            Move = MoveIndicator.GetComponent<Transform>();

            for (int i = 0; i < 6; i++)
            {
                ArrowScale[i] = ((ZDGameRule.UNIT_IN_WORLD / 3) * i);
            }
            MoveIndicator.transform.localScale = new Vector3(ZDGameRule.UNIT_IN_WORLD, ZDGameRule.UNIT_IN_WORLD, 1);
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

        private void OnDestroy()
        {
	        if(Instance == this){
            	Instance = null;
	        }
        }

    }
}

