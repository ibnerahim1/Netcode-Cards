using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnNetworkObjects : MonoBehaviour
{
    public GlobalStateSO spawner;
    public List<GameObject> networkObjects;

    [Button(nameof(SpawnObjects))]
    public bool buttonBool;

    private void Awake()
    {
        //spawner.OnEnterState += SpawnObjects;
        SpawnObjects();
    }

    public void SpawnObjects()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        for (int i = 0; i < networkObjects.Count; i++)
        {
             Instantiate(networkObjects[i]);
        }
    }
}
