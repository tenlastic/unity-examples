using kcp2k;
using Mirror;
using System;
using Tenlastic;
using UnityEngine;

public class RunAsServer : MonoBehaviour {

    [SerializeField] private KcpTransport kcpTransport = null;
    [SerializeField] private NetworkManager networkManager = null;

    private void Start() {
        if (NetworkServer.active)
            return;

        // Check if this instance was started as a server.
        EnvironmentVariables environmentVariables = EnvironmentVariables.singleton;
        if (!environmentVariables.isServer)
            return;

        try {
            kcpTransport.Port = Convert.ToUInt16(environmentVariables.gameServerModel.port);
            networkManager.StartServer();
        } catch (Exception e) {
            Debug.LogError(e);
            Application.Quit();
        }
    }

}
