using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] private Transform spawnedObjectPrefab;


    private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<MyCustomData> customNetworkedData = new NetworkVariable<MyCustomData>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



    Transform spawnedObjctTransform;

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public Vector3 _pos;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _pos);
            serializer.SerializeValue(ref message);
        }
    }



    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log("old value: " + previousValue + " new value: " + newValue);
        };
    }


    // Update is called once per frame
    void Update()
    {
 //       Debug.Log(OwnerClientId + ": " + randomNumber.Value);
        if (!IsOwner) return;


        if (Input.GetKeyDown(KeyCode.E))
        {
            spawnedObjctTransform = Instantiate(spawnedObjectPrefab, transform.position + Vector3.up * 2.0f, Quaternion.identity);

            spawnedObjctTransform.GetComponent<NetworkObject>().Spawn(true);

        }


        if (Input.GetKeyDown(KeyCode.Y))
        {
            spawnedObjctTransform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(spawnedObjctTransform);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = Random.Range(0, 100);
        }

        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        moveDir.Normalize();

        float moveSpeed = 3f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }
}
