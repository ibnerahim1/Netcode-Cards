using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using System.Linq;
using Newtonsoft.Json;

public class N_CardsListSync : NetworkBehaviour
{
    public CardsListSO cardsList;

    private void OnEnable()
    {
        if (!NetworkManager.Singleton.IsHost)
            return;

        Spawn();
    }

    public bool debugInfo = false;

    public override void OnNetworkSpawn()
    {
        cardsList.OnAddElement += OnAddElement;
        cardsList.OnRemoveElement += OnRemoveElement;
        cardsList.OnMoveElement += OnMoveElement;
        cardsList.OnRemoveAllElements += OnRemoveAllElements;
        if (NetworkManager.Singleton.IsClient)
        {
            if (debugInfo) print("Sync the cards, now I have " + cardsList.ToString());
            RequestCardsServerRpc();
        }
    }

    private void OnAddElement(int index, CardSO newCard, ComponentMetaData C_metaData)
    {
        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate)
            return;
        string C_metaDataJson = JsonUtility.ToJson(C_metaData);
        OnAddElementServerRpc(index, newCard.name, C_metaDataJson);
    }
    private void OnRemoveElement(int index, ComponentMetaData C_metaData)
    {
        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate)
            return;
        string C_metaDataJson = JsonUtility.ToJson(C_metaData);
        OnRemoveElementServerRpc(index, C_metaDataJson);
    }
    private void OnRemoveAllElements(ComponentMetaData C_metaData)
    {
        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate)
            return;
        string C_metaDataJson = JsonUtility.ToJson(C_metaData);
        OnRemoveAllElementsServerRpc(C_metaDataJson);
    }
    private void OnMoveElement(int index1, int index2, ComponentMetaData C_metaData)
    {
        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate)
            return;
        string C_metaDataJson = JsonUtility.ToJson(C_metaData);
        OnMoveElementServerRpc(index1, index2, C_metaDataJson);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestCardsServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        var cardsJson = cardsList.ToString();
        RespondCardsSyncClientRpc(cardsJson,
                new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x == serverRpcParams.Receive.SenderClientId).ToArray() } });
        if (debugInfo) print("RequestCardsServerRpc cards=" + cardsList.ToString());
    }

    [ClientRpc]
    public void RespondCardsSyncClientRpc(string cardsJson, ClientRpcParams serverRpcParams = default)
    {
        cardsList.CreateFromJson(cardsJson);
        if (debugInfo) print("RespondCardsSyncClientRpc cardsJson=" + cardsJson);
        if (debugInfo) print("RespondCardsSyncClientRpc cardsList=" + cardsList.ToString());
    }


    [ServerRpc(RequireOwnership = false)]
    public void OnAddElementServerRpc(int index, string cardUniqueName, string C_metaDataJson, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        OnAddElementClientRpc(index, cardUniqueName, C_metaDataJson, senderClientId,
                new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray() } });
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnRemoveElementServerRpc(int index, string C_metaDataJson, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        OnRemoveElementClientRpc(index, C_metaDataJson, senderClientId,
                new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray() } });
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnRemoveAllElementsServerRpc(string C_metaDataJson, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        OnRemoveAllElementsClientRpc(C_metaDataJson, senderClientId,
                new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray() } });
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnMoveElementServerRpc(int index1, int index2, string C_metaDataJson, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        OnMoveElementClientRpc(index1, index2, C_metaDataJson, senderClientId,
                new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray() } });
    }

    [ClientRpc]
    public void OnAddElementClientRpc(int index, string cardUniqueName, string C_metaDataJson, ulong senderClientId, ClientRpcParams clientRpcParams = default)
    {
        ComponentMetaData C_metaData = JsonUtility.FromJson<ComponentMetaData>(C_metaDataJson);
        CardSO newCard = CardSO.GetByName(cardUniqueName);
        C_metaData.typeOfUpdate = ComponentMetaData.TypeOfUpdate.NetworkUpdate;
        cardsList.AddElement(index, newCard, C_metaData);
    }
    [ClientRpc]
    public void OnRemoveElementClientRpc(int index, string C_metaDataJson, ulong senderClientId, ClientRpcParams clientRpcParams = default)
    {
        ComponentMetaData C_metaData = JsonUtility.FromJson<ComponentMetaData>(C_metaDataJson);
        C_metaData.typeOfUpdate = ComponentMetaData.TypeOfUpdate.NetworkUpdate;
        cardsList.RemoveElement(index, C_metaData);
    }
    [ClientRpc]
    public void OnRemoveAllElementsClientRpc(string C_metaDataJson, ulong senderClientId, ClientRpcParams clientRpcParams = default)
    {
        ComponentMetaData C_metaData = JsonUtility.FromJson<ComponentMetaData>(C_metaDataJson);
        C_metaData.typeOfUpdate = ComponentMetaData.TypeOfUpdate.NetworkUpdate;
        cardsList.RemoveAllElements(C_metaData);
    }
    [ClientRpc]
    public void OnMoveElementClientRpc(int index1, int index2, string C_metaDataJson, ulong senderClientId, ClientRpcParams clientRpcParams = default)
    {
        ComponentMetaData C_metaData = JsonUtility.FromJson<ComponentMetaData>(C_metaDataJson);
        C_metaData.typeOfUpdate = ComponentMetaData.TypeOfUpdate.NetworkUpdate;
        cardsList.MoveElement(index1, index2, C_metaData);
    }


    [Button(nameof(Spawn))]
    public bool buttonBool;

    public void Spawn()
    {
        GetComponent<NetworkObject>().Spawn();
    }
}