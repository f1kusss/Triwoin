using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag.ConnectionManagement
{
    // public enum ConnectStatus
    // {
    //     Undefined,
    //     Success, //client successfully connected. This may also be a successful reconnect.
    //     ServerFull, //can't join, server is already at capacity.
    //     LoggedInAgain, //logged in on a separate client, causing this one to be kicked out.
    //     UserRequestedDisconnect, //Intentional Disconnect triggered by the user.
    //     GenericDisconnect, //server disconnected, but no specific reason given.
    //     Reconnecting, //client lost connection and is attempting to reconnect.
    //     IncompatibleBuildType, //client build type is incompatible with server.
    //     HostEndedSession, //host intentionally ended the session.
    //     StartHostFailed, // server failed to bind
    //     StartClientFailed // failed to connect to server and/or invalid network endpoint
    // }
    //
    // public struct ReconnectMessage
    // {
    //     public int CurrentAttempt;
    //     public int MaxAttempt;
    //
    //     public ReconnectMessage(int currentAttempt, int maxAttempt)
    //     {
    //         CurrentAttempt = currentAttempt;
    //         MaxAttempt = maxAttempt;
    //     }
    // }
    //
    // public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    // {
    //     public ConnectStatus ConnectStatus;
    //     public FixedPlayerName PlayerName;
    // }
    //
    [Serializable]
    public class ConnectionPayload
    {
        // public string playerId;
        public string playerName;
        public bool isRed = true;
        public bool isDebug;
    }

    public class ConnectionManager : MonoBehaviour
    {
        public NetworkManager NetworkManager => NetworkManager.Singleton;

        [SerializeField] int m_NbReconnectAttempts = 2;

        public int NbReconnectAttempts => m_NbReconnectAttempts;
        public int MaxConnectedPlayers = 8;
        const int k_MaxConnectPayload = 1024;

        [SerializeField] private GameObject playerPrefabRed;
        [SerializeField] private GameObject playerPrefabBlue;
        [SerializeField] private GameObject playerPrefab;

        private Dictionary<ulong, ConnectionPayload> _playersClientsIdToConnectionPayload =
            new Dictionary<ulong, ConnectionPayload>();

        [SerializeField] private List<ConnectionPayload> listPlayersConnectionPayload = new List<ConnectionPayload>();

        public Action MatchStarted;


        public void ConnectAsHost(string playerName)
        {
            Debug.Log($"ConnectionManager.ConnectAsHost: {playerName}");

            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                // playerId = playerId,
                playerName = playerName,
                isRed = true,
                isDebug = Debug.isDebugBuild
            });

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            NetworkManager.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.StartHost();
            MatchStarted?.Invoke();
        }

        public void ConnectAsClient(string playerName)
        {
            Debug.Log($"ConnectionManager.ConnectAsClient: {playerName}");

            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                // playerId = playerId,
                playerName = playerName,
                isRed = true,
                isDebug = Debug.isDebugBuild
            });

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            NetworkManager.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.StartClient();
            MatchStarted?.Invoke();
        }

        public void Shutdown()
        {
            if (NetworkManager.IsHost)
                NetworkManager.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Shutdown();

            Application.Quit();
        }

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            Debug.Log($"ConnectionManager.ApprovalCheck");

            // The client identifier to be authenticated
            var clientId = request.ClientNetworkId;
            Debug.Log($"request, response:{request}, {response}");

            // Additional connection data defined by user code
            var connectionData = request.Payload;
            if (connectionData.Length > k_MaxConnectPayload)
            {
                // If connectionData too high, deny immediately to avoid wasting time on the server. This is intended as
                // a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
                response.Approved = false;
                return;
            }

            var payload = System.Text.Encoding.UTF8.GetString(connectionData);
            var connectionPayload =
                JsonUtility.FromJson<ConnectionPayload>(
                    payload); // https://docs.unity3d.com/2020.2/Documentation/Manual/JSONSerialization.html

            if (connectionPayload.isRed)
            {
                playerPrefab = playerPrefabRed;
            }
            else
            {
                playerPrefab = playerPrefabBlue;
            }

            _ = CreateCustomPlayerObjectAsync(clientId, playerPrefab);

            response.Approved = true;
            response.CreatePlayerObject = false;

            // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
            // response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            // response.Position = Vector3.zero;
            //
            // // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            // response.Rotation = Quaternion.identity;

            // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
            // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
            response.Reason = "Some reason for not approving the client";

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            response.Pending = false;

            _playersClientsIdToConnectionPayload[clientId] = connectionPayload;
            // listPlayersConnectionPayload.Add(connectionPayload);
        }

        private async Task CreateCustomPlayerObjectAsync(ulong clientId, GameObject playerPrefab)
        {
            Debug.Log($"ConnectionManager.CreateCustomPlayerObjectAsync");
            await Task.Delay(1000);

            NetworkObject playerObj = GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                .GetComponent<NetworkObject>();
            playerObj.SpawnAsPlayerObject(clientId, true);
        }
    }
}