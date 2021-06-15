using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerCustom : NetworkManager {

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);
        GameObject[] players = new GameObject[2];
    //public override void OnServerReady(NetworkConnection conn) {

    //public override void OnServerConnect(NetworkConnection conn) {
/*
        GameObject playerObject = Instantiate(playerPrefab);
        NetworkServer.Spawn(playerObject, conn);

        //PlayerController player = conn.identity.gameObject.GetComponent<PlayerController>();
        UserController player = playerObject.GetComponent<UserController>();
        GameObject[] characters = player.GetCharacters();
        foreach (GameObject character in characters) {
            CharacterController charController = character.GetComponent<CharacterController>();
            CharacterManager.instance.AddCharacter(charController);
            SpawnCharacter(character, conn);
        }
*/


        int playersNumber = NetworkServer.connections.Count;
        if (playersNumber == Const.PLAYER_NUMBER) {
            int i = 0;
            foreach (int key in NetworkServer.connections.Keys) {
                //UserController player = NetworkServer.connections[key].identity.gameObject.GetComponent<UserController>();
                //player.LoadCharacters();
                players[i] = NetworkServer.connections[key].identity.gameObject;
                i++;
            }

            GameManager.instance.PlayersReady(players);







            //GameManager.instance.StartGame();
            //UserController player1 = NetworkServer.connections[0].identity.GetComponent<UserController>();
            //UserController player2 = NetworkServer.connections[1].identity.gameObject.GetComponent<UserController>();
            //GameManager.instance.PlayersReady(NetworkServer.connections[0].identity, NetworkServer.connections[1].identity);
            //Debug.Log(player1);
            //Debug.Log(player2);
        }
    }

    /*
    public void SpawnCharacter(GameObject character, NetworkConnection owner) {
        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        Debug.Log(character);
        Debug.Log(owner);
        GameObject charObject = Instantiate(character);
        NetworkServer.Spawn(charObject, owner);
    }
    */
}
