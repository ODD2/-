using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict;
using Photon.Pun;
using ZoneDepict.Rule;

public abstract class ItemContainerBase : StationaryMapObject, IADamageObject, IPunObservable
{
    #region Field
    public float Durability;
    //[SerializeField]
    public AudioClip BrokenAudio;
    [SerializeField]
    protected GameObject BrokenEffect;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;
    #endregion

    #region Unity
    protected new void Start()
    {
        base.Start();
        //Setup Depth
        Vector3 NewPos = transform.position;
        NewPos.z = (int)TypeDepth.ItemContainer;
        transform.position = NewPos;
        //Setup Component
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Durability = 10;
    }
    protected new void Update()
    {
        base.Update();
    }

    protected new void OnDestroy()
    {
        base.OnDestroy();
    }
    #endregion

    #region Photun View
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    #endregion

    #region Item Container Interface
    public abstract void Hurt(float damaged);
    public abstract void Broken();
    #endregion

    #region Enumerator
    protected IEnumerator Vanish()
    {
        if (spriteRenderer)
        {
            while (spriteRenderer.color.a > float.Epsilon)
            {
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color -= new Color(0, 0, 0, 0.5f);
            }
        }   
    }
    #endregion

}