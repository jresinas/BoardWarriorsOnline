using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class GameServerNetworkManager : NetworkManager {

    GameObject[] players = new GameObject[Const.PLAYER_NUMBER];
    
    /*
    void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    */

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);
        Debug.Log("Player " + conn + " se ha unido al juego");


        ServerChangeScene("Game");



        int playersNumber = NetworkServer.connections.Count;
        Debug.Log("Player number: "+playersNumber);
        if (playersNumber == Const.PLAYER_NUMBER) {
            StartCoroutine(WaitStarting());
        }
    }


    IEnumerator WaitStarting() {
        yield return new WaitForSeconds(5);
        int i = 0;
        foreach (int key in NetworkServer.connections.Keys) {
            players[i] = NetworkServer.connections[key].identity.gameObject;
            i++;
        }
        Debug.Log("All players ready!!");
        GameManager.instance.PlayersReady(players);
    }
    /*
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Game") {
            if (connectType == 0) StartHost();
            else StartClient();
        }
    }
    */
}
