//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using Unity.Netcode;
//using System.Linq;

//public class N_GlobalStateSync : NetworkBehaviour
//{
//    public GlobalStateSO globalState;

//    private void Start()
//    {
//        NetworkManager.Singleton.OnServerStarted += Spawn;
//    }

//    public override void OnNetworkSpawn()
//    {
//        globalState.OnEnterState += this.OnEnterState;
//        globalState.OnExitState += this.OnExitState;
//    }

//    private void OnEnterState(ComponentMetaData C_metaData)
//    {
//        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate) return;
//        OnEnterStateServerRpc(C_metaData);
//    }
//    private void OnExitState(ComponentMetaData C_metaData)
//    {
//        if (C_metaData.typeOfUpdate == ComponentMetaData.TypeOfUpdate.NetworkUpdate) return;
//        OnExitStateServerRpc(C_metaData);
//    }

//    [ServerRpc(RequireOwnership = false)]
//    public void OnEnterStateServerRpc(ComponentMetaData C_metaData, ServerRpcParams serverRpcParams = default)
//    {
//        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
//        OnEnterStateClientRpc(C_metaData, senderClientId,
//                new ClientRpcParams
//                {
//                    Send = new ClientRpcSendParams
//                    {
//                        TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray()
//                    }
//                }
//            );
//    }
//    [ServerRpc(RequireOwnership = false)]
//    public void OnExitStateServerRpc(ComponentMetaData C_metaData, ServerRpcParams serverRpcParams = default)
//    {
//        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
//        OnExitStateClientRpc(C_metaData, senderClientId,
//                new ClientRpcParams
//                {
//                    Send = new ClientRpcSendParams
//                    {
//                        TargetClientIds = NetworkManager.Singleton.ConnectedClientsIds.Where(x => x != serverRpcParams.Receive.SenderClientId).ToArray()
//                    }
//                }
//            );
//    }

//    [ClientRpc]
//    public void OnEnterStateClientRpc(ComponentMetaData C_metaData, ulong senderClientId, ClientRpcParams clientRpcParams = default)
//    {
//        globalState.EnterState(C_metaData);
//    }
//    [ClientRpc]
//    public void OnExitStateClientRpc(ComponentMetaData C_metaData, ulong senderClientId, ClientRpcParams clientRpcParams = default)
//    {
//        globalState.ExitState(C_metaData);
//    }

//    [Button(nameof(Spawn))]
//    public bool buttonBool;

//    public void Spawn()
//    {
//        GetComponent<NetworkObject>().Spawn();
//    }
//}
