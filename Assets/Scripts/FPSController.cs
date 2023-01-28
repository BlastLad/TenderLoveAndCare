using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : PortalTraveller
{

    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;
    float verticalVelocity;
    Vector3 velocity;
    [SerializeField]
    Transform currentCam;
    int num = 0;
    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        if (GetComponent<BlastladCharacterController>())
        {
            Debug.Log("BLASTLAD caracter gotten");
            GetComponent<BlastladCharacterController>().Motor.SetPositionAndRotation(pos, rot);
            if (num == 0) {
                GetComponent<BlastladCharacterController>().Gravity = new Vector3(-30, 0, 0);
                num++;
            }
            else if (num == 1)
            {
                GetComponent<BlastladCharacterController>().Gravity = new Vector3(0, -30, 0);
                num--;
            }
            currentCam.rotation = rot;
        }
        else
        {
            transform.position = pos;
            Vector3 eulerRot = rot.eulerAngles;
            float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
            yaw += delta;
            smoothYaw += delta;
            transform.eulerAngles = Vector3.up * smoothYaw;
            velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
            Physics.SyncTransforms();
        }
    }
}
