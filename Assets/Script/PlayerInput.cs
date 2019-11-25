using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is deal with all Input from Player
// and all Input is base on 'touch' with mobile device
public class PlayerInput : MonoBehaviour
{
    //some class variable
    private Touch touch;
    
    // Start is called before the first frame update
    void Start()
    {
       
    }
    
    // Update is called once per frame
    void Update()
    {
        // Must have somefingers touch, and try one touched
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            
        }
        else if (Input.touchCount != 0 && Input.touchCount > 1)
        {
            // Multiple touched
        }
        else
        {
            // There is no touched
        }
    }

    /*
     This function will called when Player activate Attact
     rule, and with some parameters :
     Attack Type : (enum { N, A, B, R})
     Attack Direction : (Vector3 position to compute Direction)
    */
    public void InputAttack(int AtkType,Vector3 AtkDir)
    {

    }
    /*
     This function will called when Player want drag charater
     , and with some parameters :
     Began position : (Vector3)
     Ended position : (Vector3)
    */
    public void InputSprintTo(Vector3 BegPos,Vector3 EndPos)
    {
        
    }
    /*
     
    */
    public void InputUseItem()
    {

    }
    /*

   */
    public void InputGetItem()
    {

    }
    /*

   */
    public void DetectPlayerAction()
    {

    }
}
