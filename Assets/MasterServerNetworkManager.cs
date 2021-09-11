using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Server {
    public int port;
    public int users;
}

public class MasterServerNetworkManager : NetworkManager {
    void Start() {
        base.Start();
        Debug.Log(SceneManager.GetActiveScene().buildIndex);
    }


    //public override void OnServerConnect(NetworkConnection conn) {
    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);
        Debug.Log("Cliente conectado");
        Debug.Log(conn);

        /*
        UserController user = conn.identity.GetComponent<UserController>();
        if (user != null) {
            user.SetGameServerPort(666);
        }
        */
    }
    
    /*
    void RequestMatch(NetworkIdentity userIdentity) {
        UserController user = userIdentity.gameObject.GetComponent<UserController>();
        if (user != null) {
            Debug.Log(user);
            user.gameServerPort = 666;
        } 
    }
    */
}
