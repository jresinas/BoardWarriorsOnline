using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerCustom : NetworkManager {

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);
        GameObject[] players = new GameObject[Const.PLAYER_NUMBER];

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
}
