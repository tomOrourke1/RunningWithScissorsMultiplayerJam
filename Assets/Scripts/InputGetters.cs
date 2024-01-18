using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputGetters : MonoBehaviour
{
    [SerializeField] Camera cam;


    GameActions gameActions;

    public GameActions.PlayerActions Player
    {
        get { return gameActions.Player; }
    }

    public Vector3 PlannarInput
    {
        get
        {
            Vector3 right = cam.transform.right;
            Vector3 forward = cam.transform.forward;
            right.y = 0;
            forward.y = 0;
            right.Normalize();
            forward.Normalize();

            var inp = gameActions.Player.MoveAxis.ReadValue<Vector2>();

            return (forward * inp.y + right * inp.x).normalized;
        }
    }

    private void Awake()
    {
        gameActions = new GameActions();
        gameActions.Enable();

    }



}
