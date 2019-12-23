using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ZoneDepict;
using ZoneDepict.Rule;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    [Header("Objects of Bar")]
    public Image HealthBar;
    public Image ManaBar;
    [Header("Object of AngleIndicator")]
    public GameObject TrackIndicator;

    [Header("Object of Soul")]
    public Sprite SoulImgSource;
    public GameObject Soul;
    [Header("Green and Red HP Bar")]
    public Sprite HPBar_G;
    public Sprite HPBar_R;
    protected Character Owner;

    //About Soul
    private int SoulDisplayed = 0;
    private GameObject[] Souls;

    private GameObject Temp;
    private GameObject HPBar;
    void Start()
    {
        Owner = gameObject.GetComponentInParent<Character>();
        HPBar = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        if (Owner == null)
        {
            Destroy(gameObject);
        }
        if (!Owner.photonView.IsMine)
        {
            Destroy(TrackIndicator);
        }
        //Setup UI Layer.
        Vector3 Pos = transform.position;
        Pos.z = (float)GameActorLayers.CharacterInfo;
        transform.position = Pos;

        //Set Name
        gameObject.name = Owner.name + "'s UI Object";

        //Listen To Event
        Owner.ShelterStateChanged += ShelterStateUpdated;

        //Setup Soul (Cache)
        Souls = new GameObject[5];
        for (int i = 0; i < 5; ++i)
        {
            Souls[i] = Soul.transform.GetChild(i).gameObject;
            Souls[i].GetComponent<Image>().sprite = SoulImgSource;
            Souls[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }


        Temp = gameObject.transform.gameObject.transform.GetChild(0).gameObject;
        if(Owner && Owner.photonView.Owner.CustomProperties.ContainsKey("Team"))
        {
            if ((int)Owner.photonView.Owner.CustomProperties["Team"] == 0)
            {
                SetHPColor("Green");
            }
            else
            {
                SetHPColor("Red");
            }
        }
        
    }
    
    protected void ShelterStateUpdated(object sender, ZDObject.ShelterStateChangeArgs args)
    {
        if (args.InShelter && !Owner.photonView.IsMine)
        {
            Temp.SetActive(false);
        }
        else
        {
            Temp.SetActive(true);
        }
    }

    public void UpdateHPBar(float maxHP, float HP)
    {
        //HealthBar.sizeDelta = new Vector2((maxHP / HealthBarBG.rect.width) * HP, HealthBar.rect.height);
        HealthBar.fillAmount = HP / maxHP;
    }

    public void UpdateMPBar(float maxMP, float MP)
    {
        //ManaBar.sizeDelta = new Vector2((maxMP / ManaBarBG.rect.width) * MP, ManaBar.rect.height);
        ManaBar.fillAmount = MP / maxMP;
    }

    public void SetHPColor(string color)
    {
        switch (color)
        {
            case "Red":
                HPBar.GetComponent<Image>().sprite = HPBar_R;
                break;
            case "Green":
                HPBar.GetComponent<Image>().sprite = HPBar_G;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (Owner)
        {
            if (Owner.photonView.IsMine)
            {
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

            UpdateHPBar(Owner.GetMaxHP(), Owner.GetHP());
            UpdateMPBar(Owner.GetMaxMP(), Owner.GetMP());
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            Owner.Hurt(10);
        }
    }
}
