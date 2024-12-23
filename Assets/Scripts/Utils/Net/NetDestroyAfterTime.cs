using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetDestroyAfterTime : NetworkBehaviour
{
    [SerializeField] private float _seconds = 1f;

    private void Start()
    {
        if (IsServer)
            Destroy(gameObject, _seconds);
    }
}