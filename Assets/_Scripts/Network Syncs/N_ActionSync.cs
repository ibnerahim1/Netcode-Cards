using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using System.Linq;
using Newtonsoft.Json;
    
public class N_ActionSync : NetworkBehaviour
{
    public ActionSO action;

    private void OnEnable()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        Spawn();
    }

    public override void OnNetworkSpawn()
    {
        action.OnActionPlayed += OnActionPlayed;
    }

    private void OnActionPlayed(CardSO [] cards, ComponentMetaData C_metaData)
    {
        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate) return;
        string ActionIDJson = CardSO.ListToJson(cards);
        string C_metaDataJson = JsonUtility.ToJson(C_metaData);
        OnActionPlayedServerRpc(ActionIDJson, C_metaDataJson);
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnActionPlayedServerRpc(string cardsName, string C_metaDataJson, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        OnActionPlayedClientRpc(cardsName, C_metaDataJson, senderClientId,
                new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray()
                    }
                }
            );
    }

    [ClientRpc]
    public void OnActionPlayedClientRpc(string cardsJson, string C_metaDataJson, ulong senderClientId, ClientRpcParams clientRpcParams = default)
    {
        var cards = CardSO.JsonToList(cardsJson);
        action.PlayAction(cards, ComponentMetaData.GetC_metaDataNetwork(gameObject.GetInstanceID()));
    }

    [Button(nameof(Spawn))]
    public bool buttonBool;

    public void Spawn()
    {
        GetComponent<NetworkObject>().Spawn();
    }
}
