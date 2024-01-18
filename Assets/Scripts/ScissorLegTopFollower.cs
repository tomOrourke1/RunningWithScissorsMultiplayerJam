using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScissorLegTopFollower : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] Transform leftLegTop;
    [SerializeField] Transform rightLegTop;




    private void LateUpdate()
    {
        var avePos = (leftLegTop.position + rightLegTop.position) * 0.5f;

        transform.position = avePos;

    }


}
