using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 50,
            _bool = true,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    float moveSpeed = 3f;

    public struct MyCustomData: INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = new MyCustomData {
                _int = 10,
                _bool = false,
                message = "New fone who dis",
            };
        }

        Vector3 moveDir = new Vector3();
        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
