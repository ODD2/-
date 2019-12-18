using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

class SmoothTransform : MonoBehaviour, IPunObservable
{
    public bool position=true;
    public bool rotation=false;
    public bool scale=true;
    public Vector2 diffPosBound = new Vector2(0.2f,2), 
                             diffScaleBound = new Vector2(0.2f, 2),
                             diffAngleBound = new Vector2(0.2f, 2);
    protected Vector3 DeltaPos;
    protected Vector3 DeltaScale;
    protected Vector3 DeltaAngle;

    void Start()
    {
    }

    void  Update()
    {
      
    }

     void FixedUpdate()
    {
        if (!DeltaPos.Equals(Vector3.zero))
        {
            Vector3 UpdatePos = DeltaPos * Time.deltaTime;
            DeltaPos -= UpdatePos;
            transform.position += UpdatePos;
        }
        if (!DeltaScale.Equals(Vector3.zero))
        {
            Vector3 UpdateScale = DeltaScale * Time.deltaTime;
            DeltaScale -= UpdateScale;
            transform.localScale += UpdateScale;
        }
        if (!DeltaAngle.Equals(Vector3.zero))
        {
            Vector3 UpdateAngle = DeltaAngle * Time.deltaTime;
            DeltaAngle -= UpdateAngle;
            transform.eulerAngles += UpdateAngle;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if(position) stream.SendNext(transform.position);
            if(scale) stream.SendNext(transform.eulerAngles);
            if(rotation) stream.SendNext(transform.localScale);
        }
        else if (stream.IsReading)
        {
            if (position)
            {
                DeltaPos = (Vector3)stream.ReceiveNext() - transform.position;
                float diffPos = DeltaPos.magnitude;
                if (diffPos < diffPosBound.x || diffPos > diffPosBound.y)
                {
                    transform.position += DeltaPos;
                    DeltaPos = Vector3.zero;
                }
            }
            if (scale)
            {
                DeltaAngle = (Vector3)stream.ReceiveNext() - transform.eulerAngles;
                float diffAngle = DeltaAngle.magnitude;
                if (diffAngle < diffAngleBound.x || diffAngle > diffAngleBound.x)
                {
                    transform.localScale += DeltaAngle;
                    DeltaAngle = Vector3.zero;
                }
            }
            if (rotation)
            {
                DeltaScale = (Vector3)stream.ReceiveNext() - transform.localScale;
                float diffScale = DeltaScale.magnitude;
                if (diffScale < diffScaleBound.x || diffScale > diffScaleBound.x)
                {
                    transform.localScale += DeltaScale;
                    DeltaScale = Vector3.zero;
                }
            }
        }
    }
}