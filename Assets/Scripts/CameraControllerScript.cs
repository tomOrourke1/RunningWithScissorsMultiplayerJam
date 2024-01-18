using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraControllerScript : NetworkBehaviour
{
    [Header("Dependencies")]
    [SerializeField] InputGetters input;
    [SerializeField] Transform pivotPitch;
    [SerializeField] Transform pivotYaw;

    [Header("values")]
    [SerializeField] float yawSpeed;
    [SerializeField] float pitchSpeed;
    [Header("MouseValues")]
    [SerializeField] float mYawSpeed;
    [SerializeField] float mPitchSpeed;


    [Header("Keyboard or Controller")]
    [SerializeField] bool controllerOn = false;

    float angleYaw;
    float anglePitch;



    private void FixedUpdate()
    {
        if(!IsOwner) return;

        var inp = Vector2.zero;
        float deltaYaw = 0;
        float deltaPitch = 0;

        if (controllerOn)
        {
            inp = input.Player.LookAxis.ReadValue<Vector2>();
            deltaYaw = inp.x * yawSpeed * Time.deltaTime;
            deltaPitch = inp.y * pitchSpeed * Time.deltaTime;
        }
        else
        {
            inp = input.Player.MouseDelta.ReadValue<Vector2>();
            deltaYaw = inp.x * mYawSpeed * Time.deltaTime;
            deltaPitch = inp.y * mPitchSpeed * Time.deltaTime;
        }





        angleYaw += deltaYaw;
        anglePitch -= deltaPitch;



        angleYaw = (angleYaw >= 360) ? angleYaw - 360 : angleYaw;
        anglePitch = Mathf.Clamp(anglePitch, -75, 75);


        pivotYaw.localRotation = Quaternion.Euler(0, angleYaw, 0);
        pivotPitch.localRotation = Quaternion.Euler(anglePitch, 0, 0);
    }

}
