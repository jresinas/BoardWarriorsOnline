using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using kcp2k;


public class UserController : NetworkBehaviour {
    [SerializeField] GameObject[] characters;
    //public bool isTurn = false;
    [SyncVar] public int gameServerPort = 0;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadCharacters(int player) {
        Debug.Log("UserController:LoadCharacters");
        int characterIndex = 0;
        foreach (GameObject character in characters) {
            Debug.Log(character);
            GameObject charObject = Instantiate(character);
            NetworkServer.Spawn(charObject, netIdentity.connectionToClient);
            Vector2Int position = new Vector2Int(characterIndex, player == 0 ? (Const.BOARD_ROWS-1) : 0);
            CharacterManager.instance.AddCharacter(charObject.transform, position, player);
            characterIndex++;
        }
    }






    private void Update() {
        if (Input.GetKeyDown("space")) RequestGameServerPort();
    }

/*
    public void SetGameServerPort(ushort port) {
        Debug.Log("SetGameServerPort");
        gameServerPort = port;
        ConnectGameServer(port);
    }
*/

    [Command]
    public void RequestGameServerPort() {
        Debug.Log("RequestGameServerPort");
        SetGameServerPort(connectionToClient, 7800);
        //SetGameServerPort(7800);
    }

    [TargetRpc]
    public void SetGameServerPort(NetworkConnection conn, ushort port) {
        Debug.Log("SetGameServerPort-2");
        gameServerPort = port;
        ConnectGameServer(port);
    }

    void ConnectGameServer(ushort port) {
        KcpTransport transport = NetworkManager.singleton.GetComponent<KcpTransport>();
        if (transport != null) {
            NetworkManager.singleton.StopClient();
            transport.Port = port;
            NetworkManager.singleton.onlineScene = "Game";
            NetworkManager.singleton.StartClient();
            Debug.Log("Change scene");
            //SceneManager.LoadScene("Game");
            //NetworkManager.singleton.ServerChangeScene("Game");
        }
    }
}
