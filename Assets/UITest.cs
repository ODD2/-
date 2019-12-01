using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict.Rule;
public class UITest : MonoBehaviour
{
    public Text txt;
    public Button But;
    public GameObject Circle;
    private GameObject Obj;
    // Start is called before the first frame update
    void Start()
    {
        But.onClick.AddListener(() => { Onclick(); });
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.touchCount > 0)
        {
            //Debug.Log("Test");
            Touch t = Input.GetTouch(0);
            Vector2 pos = Camera.main.ScreenToWorldPoint(t.position);
            if (t.phase == TouchPhase.Began)
            {
                //Obj = Instantiate(Circle);
                //Obj.GetComponent<Transform>().position = pos;
            }
            if (t.phase == TouchPhase.Ended)
            {
                //Destroy(Obj);
            }
            if ((t.phase == TouchPhase.Began) && ZDGameRule.CalculateDistance(pos, new Vector2(0, 0)) < 2)
            {
                //Debug.Log("Hit Zero Point");
            }
        }
    }
    public void Onclick()
    {
        //Debug.Log("Button Click~");
    }
}
