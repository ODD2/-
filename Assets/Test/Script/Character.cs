using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using ZoneDepict;

public class Character : ZDObject, IPunObservable
{
    #region Private Field
    protected float HP { get; private set; } = 100;
    #endregion

    #region UNITY
    private void Awake()
    {

    }
    //item 測試用
    public void GetItem(ItemBase  i)
    {
        Debug.LogFormat("Player get {0} ",i);
    }

    // Start is called before the first frame update
    private new void Start()
    {
        //Trigger ZDObject Routine
        base.Start();
    }

    // Update is called once per frame
    private new void Update()
    {
        //Trigger ZDObject Routine
        base.Update();

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Current Health:" + HP);
        }
        if (Input.GetKeyDown(KeyCode.Z) && photonView.IsMine)
        {
            Vector2 delta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            photonView.RPC("SimpleAttack", RpcTarget.AllViaServer, delta);
        }
        //滑鼠點擊道具偵測
        if (Input.GetMouseButtonDown(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 10, -1); ;
            if (hit.collider)
            {
                Debug.Log(hit.collider.name);
                if (hit.collider.gameObject.GetComponent<DropItem>())//掉落物
                {
                    Debug.Log("hit drop item");
                    hit.collider.gameObject.GetComponent<DropItem>().AddToPlayer(this);
                }
                else if (hit.collider.gameObject.GetComponent<RandomItemContainer>()) //寶箱
                {
                    Debug.Log("hit random box");
                    hit.collider.gameObject.GetComponent<RandomItemContainer>().Damaged(1000.0f);

                }

            }


        }
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
    }

    private void FixedUpdate()
    {
    }

    private void OnMouseDown()
    {

    }

    private void OnMouseDrag()
    {

    }

    private void OnMouseUp()
    {
        //只有擁有者可以操控這隻角色
        if (photonView && photonView.IsMine)
        {
            var MouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = GameSetting.WorldUnify(new Vector3(MouseWorldPosition.x, MouseWorldPosition.y, transform.position.z));
        }
    }
    #endregion

    #region Observable
    //Synchronize Health/Position
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HP);
        }
        else if (stream.IsReading)
        {
            transform.position = (Vector3)stream.ReceiveNext();
            HP = (float)stream.ReceiveNext();
        }
    }
    #endregion

    #region Private Method
    private void DoSimpleAttack(Vector2 Direction)
    {

        Vector2 UnitOffset = GameSetting.QuadDirection(Direction);
        //search in the ZDMap whom was hit.
        List<ZDObject> HitObject = ZDMap.HitAt(UnitOffset, this);

        Debug.Log("DoSimpleAttack - PlayerLocation: " + MapLocX + "," + MapLocY);
        Debug.Log("DoSimpleAttack - HitDirection: " + UnitOffset.x + "," + UnitOffset.y);

        //if there's a list
        if (HitObject != null)
        {
            foreach (var Obj in HitObject)
            {
                if (Obj is Character)
                {
                    ((Character)Obj).ReceiveDamage(10);
                }
                //else if(Obj is ItemContainer)
                //{

                //}
            }
        }
    }
    #endregion

    public void ReceiveDamage(float damage)
    {
        if (photonView.IsMine)
        {
            HP = damage > HP ? 0 : HP - damage;
        }
    }

    #region PUN Callback & RPC
    [PunRPC]
    public void TEST()
    {
        Debug.Log("Received Testa Rpc.");
    }

    //[PunRPC]
    //public void SprintTo(Vector3  UnitLoc,PhotonMessageInfo info)
    //{
    //    //所有人都可以更新他的Map
    //    if (ZDMap.UpdateLocation(UnitLoc, this))
    //    {
    //        //只有擁有者可以更新角色的位置
    //        if(photonView && photonView.IsMine)
    //        {
    //            transform.position = GameSetting.UnitToWorld((int)UnitLoc.x, (int)UnitLoc.y, 0);
    //        }
    //    }
    //}

    [PunRPC]
    public void SimpleAttack(Vector2 Direction, int test)
    {
        DoSimpleAttack(Direction);
    }
    #endregion
}
