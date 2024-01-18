using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] InputGetters input;
    [SerializeField] Transform pivotPitch;
    [SerializeField] Transform pivotYaw;

    [Header("values")]
    [SerializeField] float yawSpeed;
    [SerializeField] float pitchSpeed;


    float angleYaw;
    float anglePitch;



    private void FixedUpdate()
    {
        var inp = input.Player.LookAxis.ReadValue<Vector2>();

        float deltaYaw = inp.x * yawSpeed * Time.deltaTime;
        float deltaPitch = inp.y * pitchSpeed * Time.deltaTime;


        angleYaw += deltaYaw;
        anglePitch -= deltaPitch;

        angleYaw = (angleYaw >= 360) ? angleYaw - 360 : angleYaw;
        anglePitch = Mathf.Clamp(anglePitch, -75, 75);
        

        pivotYaw.localRotation = Quaternion.Euler(0, angleYaw, 0);
        pivotPitch.localRotation = Quaternion.Euler(anglePitch, 0, 0);
    }

}
