﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZoneDepict;

public class VisualCountDown : MonoBehaviour
{
    private Image Img;
    private ZDGameManager GameManager;
    private int MaxCountTime;
    private int CountDownTime;
    private float RemainTime;
    private GameObject WhiteBG;
    [Header("CountDown Numbers")]
    public GameObject[] CountDownCircleView;
    

    void Start()
    {
        Img = GetComponent<Image>();
        WhiteBG = GameObject.Find("WhiteBG");
        Debug.Log(WhiteBG);
        GameManager = GameObject.Find("GameManager").GetComponent<ZDGameManager>();
        MaxCountTime = GameManager.CountDownTime+1;
        CountDownTime = GameManager.CountDownTime+1;
        RemainTime = MaxCountTime-1;
        MaxCountTime -= 1;
        StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update()
    {
        if (Img == null || GameManager == null)
            Destroy(this);
    }
    private void FixedUpdate()
    {
        RemainTime -= Time.fixedDeltaTime;
        Img.fillAmount = RemainTime / MaxCountTime;
    }
    IEnumerator CountDown()
    {
        while(CountDownTime > 0)
        {
            Debug.Log(CountDownTime);
            CountDownCircleView[(int)CountDownTime - 1].SetActive(true);
            yield return new WaitForSeconds(1);
            CountDownTime -= 1;
            CountDownCircleView[(int)CountDownTime].SetActive(false);
        }
        Destroy(WhiteBG);
        Destroy(this.gameObject);
    }
}
