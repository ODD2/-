using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeachingView : MonoBehaviour
{
    public Sprite[] TeachImg;
    public Image View;
    public GameObject Left;
    public GameObject Right;

    private int index = 0;

    void Start()
    {
        index = 0;
        Left.GetComponent<Button>().onClick.AddListener(() => ToLeft());
        Right.GetComponent<Button>().onClick.AddListener(() => ToRight());
        View.sprite = TeachImg[index];
        UpdateBut();
    }

    
    void Update()
    {
        
    }

    void ToLeft()
    {
        if (index > 0)
            index -= 1;
        UpdateBut();
        View.sprite = TeachImg[index];
    }
    void ToRight()
    {
        if (index < 6)
            index += 1;
        UpdateBut();
        View.sprite = TeachImg[index];
    }
    void UpdateBut()
    {
        if (index == 0) Left.SetActive(false);
        else Left.SetActive(true);
        if (index == TeachImg.Length-1) Right.SetActive(false);
        else Right.SetActive(true);
    }
}
