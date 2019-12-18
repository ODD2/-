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
    public GameObject TrackAngleIndicator;

    [Header("Object of Soul")]
    public Sprite SoulImgSource;
    public GameObject Soul;

    protected Character Owner;

    //About Soul
    private int SoulDisplayed = 0;
    private GameObject[] Souls;

    private GameObject Temp; 
    void Start()
    {
        Owner = gameObject.GetComponentInParent<Character>();

        if (Owner == null) Destroy(this.gameObject);
        if (!Owner.photonView.IsMine) Destroy(TrackAngleIndicator);

        Owner.ShelterStateChanged += ShelterStateUpdated;
        gameObject.name = Owner.name + "'s UI Object";
        Souls = new GameObject[5];
        for (int i = 0; i < 5; ++i)
        {
            Souls[i] = Soul.transform.GetChild(i).gameObject;
            Souls[i].GetComponent<Image>().sprite = SoulImgSource;
            Souls[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        
        Temp = gameObject.transform.gameObject.transform.GetChild(0).gameObject;
        
        //TrackAngleIndicator.SetActive(false);
        if (Owner is CrossTrackCharacter CrossTrackOwner)
        {
            CrossTrackOwner.TrackAngleIndicator = TrackAngleIndicator;
            CrossTrackOwner.TrackAngleIndicator.SetActive(false);
        }
        
        
    }

    // Update is called once per frame
    
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

    private void Update()
    {
        if (Owner)
        {
            if (Owner.GetComponent<PhotonView>().IsMine)
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
            UpdateHPBar(Owner.GetMaxHP(), Owner.GetHP());
            UpdateMPBar(Owner.GetMaxMP(), Owner.GetMP());
        }
        if(Input.GetKeyDown(KeyCode.A))
        {
            Owner.Hurt(10);
        }
    }
}
