using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerManager : NetworkBehaviour {
    public static ServerManager instance;

    [SyncVar] public int prueba = 0;

    [SerializeField] int gameServerPort;

    void Awake() {
        instance = this;
    }

    [TargetRpc]
    public void SetGameServer(NetworkConnection conn, int port) {
    //[ClientRpc]
    //public void SetGameServer(int port) {
        Debug.Log("Puerto: " + port);
        gameServerPort = port;
    }

    [Command(requiresAuthority = false)]
    public void RequestGameServer() {
        Debug.Log("REQUESTGAMESERVER");
        Debug.Log(connectionToClient);
        SetGameServer(connectionToClient, 666);
    }





    private void Update() {
        if (Input.GetKeyDown("space")) prueba++;
    }
}
