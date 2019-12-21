using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZoneDepict.Rule;
public class TrackIndicatorController : MonoBehaviour
{
    public float SmoothScaler = 3.0f;

    public GameObject Arrow, Circle;
    // Start is called before the first frame update
    protected CrossTrackCharacter Owner;
    protected Material materialCircle;

    bool SmoothTransAngle;
    float DeltaAngle;
    float TargetAngle;
    
    void Start()
    {
        Owner = GetComponentInParent<CrossTrackCharacter>();
        if (Arrow == null ||
            Circle == null ||
            Owner  == null||
            Circle.GetComponent<SpriteRenderer>() ==null) Destroy(gameObject);

        Owner.TrackInfoChanged += UpdateTrackIndicator;

        materialCircle = Circle.GetComponent<SpriteRenderer>().material;
        //Adjust Layer
        Vector3 NewPos = transform.position;
        NewPos.z = (float)GameActorLayers.CharacterInfo;
        transform.position = NewPos;

        //Turn Off On Initialize
        ResetIndicator();
        gameObject.SetActive(false);
        
    }

    private void ResetIndicator()
    {
        Vector3 rotation = Arrow.transform.rotation.eulerAngles;
        rotation.z = 90;
        TargetAngle = 90;
        DeltaAngle = 0;
        SmoothTransAngle = false;
        Arrow.transform.rotation = new Quaternion() { eulerAngles = rotation };
    }

    void UpdateTrackIndicator(object send, CrossTrackCharacter.TrackInfoChangeArgs args)
    {
        if (!args.HasTrack)
        {
            //Circle.SetActive(false);
            //Arrow.SetActive(false);

            //Force Setup Rotation
            ResetIndicator();
            gameObject.SetActive(false);
        }
        else
        {
            if (!gameObject.activeSelf)
            {
                //Circle.SetActive(true);
                //Arrow.SetActive(true);
                gameObject.SetActive(true);
            }
            RotateToAngle(args.Angle);
        }
    }

    public void RotateToAngle(float newAngle)
    {
        if (newAngle.Equals(Arrow.transform.rotation.eulerAngles.z) || TargetAngle.Equals(newAngle))
        {
            DeltaAngle += DeltaAngle< 0 ? -359:359;
        }
        else
        {
            DeltaAngle = (newAngle - Arrow.transform.rotation.eulerAngles.z);
            //DeltaAngle += (DeltaAngle<0) ? -360: 360;
        }
        TargetAngle = newAngle;
        SmoothTransAngle = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (SmoothTransAngle)
        {
            Vector3 rotation = Arrow.transform.rotation.eulerAngles;
            float angleChanged = DeltaAngle * SmoothScaler * Time.fixedDeltaTime;
            if (Mathf.Abs(angleChanged) < 0.00001)
            {
                rotation.z += DeltaAngle;
                SmoothTransAngle = false;
            }
            else
            {  
                rotation.z += angleChanged;
                DeltaAngle -= angleChanged;
            }
            Arrow.transform.rotation = new Quaternion() { eulerAngles = rotation };
        }
        materialCircle.SetFloat("_Fill", Owner.TrackRemainTime / Owner.TrackDurationTime);
    }
}
