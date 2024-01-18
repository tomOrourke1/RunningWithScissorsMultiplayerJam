using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WindowEater : MonoBehaviour
{

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;       
    }
}
