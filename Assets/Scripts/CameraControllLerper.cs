using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraControllLerper : NetworkBehaviour
{



    private void LateUpdate()
    {
        if (!IsOwner) return;

        Camera.main.transform.SetPositionAndRotation(transform.position, transform.rotation);        
    }


}
