using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UIstate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool ishover;
    public void OnPointerEnter(PointerEventData eventData)
    {

        ishover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ishover = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        ishover = false;
    }
}
