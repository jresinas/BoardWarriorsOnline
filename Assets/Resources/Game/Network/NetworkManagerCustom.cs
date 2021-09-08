using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerCustom : NetworkManager {
    GameObject[] players = new GameObject[Const.PLAYER_NUMBER];
    // 0 = host. 1 = client.
    int connectType;

    void Awake() {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);

        int playersNumber = NetworkServer.connections.Count;
        if (playersNumber == Const.PLAYER_NUMBER) {
            int i = 0;
            foreach (int key in NetworkServer.connections.Keys) {
                players[i] = NetworkServer.connections[key].identity.gameObject;
                i++;
            }

            GameManager.instance.PlayersReady(players);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Game") {
            if (connectType == 0) StartHost();
            else StartClient();
        }
    }

    public void CreateGame() {
        connectType = 0;
        SceneManager.LoadScene("Game");
    }

    public void JoinGame() {
        connectType = 1;
        SceneManager.LoadScene("Game");
    }
}
