using kcp2k;
using Mirror;
using System;
using Tenlastic;
using UnityEngine;

public class RunAsClient : MonoBehaviour {

    [SerializeField] private KcpTransport kcpTransport = null;
    [SerializeField] private NetworkManager networkManager = null;

    private void Start() {
        if (NetworkClient.active)
            return;

        // Check if this instance was started as a client.
        EnvironmentVariables environmentVariables = EnvironmentVariables.singleton;
        if (environmentVariables.isServer)
            return;

        // Show the loading screen and connect to the server.
        GameServerModel gameServerModel = environmentVariables.gameServerModel;

        // Override Access and Refresh Tokens.
        TokenManager.singleton.accessToken = environmentVariables.accessToken;
        TokenManager.singleton.refreshToken = environmentVariables.refreshToken;

        // Set the port.
        kcpTransport.Port = Convert.ToUInt16(gameServerModel.port);

        // Set the URL.
        EnvironmentObject environment = EnvironmentManager.singleton.environmentObject;
        string host = new Uri(environment.gameServerApiBaseUrl).Host;
        Uri uri = new Uri(string.Format("tcp4://{0}", host));

        // Start the client.
        networkManager.StartClient(uri);
    }

}
