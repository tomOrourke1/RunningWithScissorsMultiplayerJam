using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{

    [Header("Components")]
    [SerializeField] SimpleMotor motor;
    [SerializeField] InputGetters input;

    [Header("Values")]
    [SerializeField] float speed = 10;
    [SerializeField] float gravity = 10;

    private void LateUpdate()
    {
        if (!IsOwner) return;
        GroundCheck();

        Vector3 vel = input.PlannarInput;


        vel = vel * speed;

        motor.Move(vel * Time.deltaTime, gravity * Time.deltaTime);
    }



    void GroundCheck()
    {
        RaycastHit hit;
        Physics.SphereCast(transform.position + Vector3.up * 0.5f, 0.5f - 0.015f, Vector3.down, out hit);

        motor.Grounded = hit.distance <= 0.5f + 0.015f;
    }


}
