using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict.Rule;
using UnityEngine.UI;

public class CharactorUI : MonoBehaviour
{
    [Header("Objects of Bar")]
    public RectTransform HealthBar;
    public RectTransform HealthBarBG;
    public RectTransform ManaBar;
    public RectTransform ManaBarBG;

    [Header("Object of Soul")]
    public Sprite SoulImgSource;
    public GameObject Soul;

    protected Character Owner;

    //About Soul
    private int SoulDisplayed = 0;
    private GameObject[] Souls;

    void Start()
    {
        Owner = gameObject.transform.parent.gameObject.GetComponent<Character>();
        gameObject.name = Owner.name + "'s UI Object";
        if (Owner.GetComponent<PhotonView>().IsMine)
        {
            Souls = new GameObject[5];
            for (int i = 0; i < 5; ++i)
            {
                Souls[i] = Soul.transform.GetChild(i).gameObject;
                Souls[i].GetComponent<Image>().sprite = SoulImgSource;
                Souls[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
        }
        else
        {
            Destroy(Soul);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && ZDController.GetTargetCharacter())
        {
            Character target = ZDController.GetTargetCharacter();
            target.Hurt(10);
            target.SetMP(target.GetMP() - 10);
            //Debug.Log(ZDController.TargetCharacter.GetSoul());
            ManaBar.sizeDelta += new Vector2(-5, 0);
            Debug.Log(ManaBar.sizeDelta);
            
            
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            Owner.SetSoul(Owner.GetSoul() - 1);
        }
        
    }

    public void UpdateHPBar(float maxHP, float HP)
    {
        HealthBar.sizeDelta = new Vector2((maxHP / HealthBarBG.rect.width) * HP, HealthBar.rect.height);
    }

    public void UpdateMPBar(float maxMP, float MP)
    {
        ManaBar.sizeDelta = new Vector2((maxMP / ManaBarBG.rect.width) * MP, ManaBar.rect.height);
    }

    
    private void FixedUpdate()
    {
        if (Owner)
        {
            UpdateHPBar(Owner.GetMaxHP(), Owner.GetHP());
            UpdateMPBar(Owner.GetMaxMP(), Owner.GetMP());

            if(Owner.GetComponent<PhotonView>().IsMine)
            {
                int GetSoul = Owner.GetSoul();
                if (SoulDisplayed != GetSoul)
                {
                    if (SoulDisplayed < GetSoul)
                    {
                        for (int i = SoulDisplayed; i < GetSoul; ++i)
                        {
                            Souls[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        }

                    }
                    else if (SoulDisplayed > GetSoul)
                    {
                        for (int i = GetSoul; i < SoulDisplayed; i++)
                        {
                            Souls[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                        }
                    }
                    SoulDisplayed = GetSoul;

                }
            }
            
        }
    }
}
