using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{

    [SerializeField] SimpleMotor motor;


    [SerializeField] float speed = 10;
    [SerializeField] float gravity = 10;

    private void LateUpdate()
    {
        if (!IsOwner) return;
        GroundCheck();

        Vector3 vel = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) vel.x = -1;
        if (Input.GetKey(KeyCode.D)) vel.x = 1;
        if (Input.GetKey(KeyCode.W)) vel.z = 1;
        if (Input.GetKey(KeyCode.S)) vel.z = -1;

        vel.Normalize();

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
