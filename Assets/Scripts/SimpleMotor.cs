using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleMotor : MonoBehaviour
{



    int maxBounces = 5;
    float skinWidth = 0.015f;
    float maxSlopeAngle = 55;

    Bounds bounds;
    [SerializeField] Collider collider;

    bool isGrounded = false;
    [SerializeField]
    LayerMask layerMask;

    [Space]
    [SerializeField] Vector3 colliderOffset;

    private void Update()
    {
        bounds = collider.bounds;
        bounds.Expand(-2 * skinWidth);
    }



    public bool Grounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    public void Move(Vector3 amount, float gravity)
    {
        amount = CollideAndSlide(amount, transform.position, 0, false, amount);
        var grav = new Vector3(0, -gravity, 0);
        amount += CollideAndSlide(grav, transform.position + amount, 0, true, grav);

        transform.position += amount;
    }


    private Vector3 CollideAndSlide(Vector3 vel, Vector3 pos, int depth, bool gravityPass, Vector3 velInit)
    {
        if (depth >= maxBounces)
        {
            return Vector3.zero;
        }


        float dist = vel.magnitude + skinWidth;
        RaycastHit hit;

        //bool dohit = Physics.CapsuleCast(pos, pos + Vector3.up * 0.1f, bounds.extents.x, vel.normalized, out hit, dist, layerMask);
        if (
            //dohit
            Physics.SphereCast(pos + colliderOffset, bounds.extents.x, vel.normalized, out hit, dist, layerMask)
            )
        {
            Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);
            Vector3 leftover = vel - snapToSurface;

            float angle = Vector3.Angle(Vector3.up, hit.normal);

            if (snapToSurface.magnitude <= skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            //normal gound / slope
            if (angle <= maxSlopeAngle)
            {
                if (gravityPass)
                {
                    return snapToSurface;
                }
                leftover = ProjectAndScale(leftover, hit.normal);
            }
            // wall or steep slope
            else
            {
                float scale = 1 - Vector3.Dot(
                    new Vector3(hit.normal.x, 0, hit.normal.z).normalized,
                    -new Vector3(velInit.x, 0, velInit.z).normalized);


                if(isGrounded && !gravityPass)
                {
                    leftover = ProjectAndScale(
                        new Vector3(leftover.x, 0, leftover.z),
                        new Vector3(hit.normal.x, 0, hit.normal.z)
                        );
                    leftover *= scale;
                }
                else
                {
                    leftover = ProjectAndScale(leftover, hit.normal) * scale;
                }
            }

            return snapToSurface + CollideAndSlide(leftover, pos + snapToSurface, depth + 1, gravityPass, velInit);

        }

        return vel;
    }


    Vector3 ProjectAndScale(Vector3 p1, Vector3 p2)
    {
        float mag = p1.magnitude;
        p1 = Vector3.ProjectOnPlane(p1, p2).normalized;
        return (p1 * mag);
    }
}
