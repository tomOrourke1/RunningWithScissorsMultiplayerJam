using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LegsAnimController : NetworkBehaviour
{
    [Header("Dependencies")]
    [SerializeField] Animator animator;
    [SerializeField] InputGetters input;



    private void LateUpdate()
    {
        if (!IsOwner) return;
        
        var inp = input.Player.MoveAxis.ReadValue<Vector2>().magnitude;
        inp = Mathf.Clamp01(Mathf.Abs(inp));

        animator.SetFloat("RunSpeed", inp);
    }

}
