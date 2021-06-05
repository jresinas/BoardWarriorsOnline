using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BeamLightController : NetworkBehaviour {
    bool moving = false;
    float moveTime = 1;
    Vector3 originPosition;
    Vector3 targetPosition;
    float time = 0;
    Vector3 mask = new Vector3(1, 0, 1);
    Vector3 offset = new Vector3(0, 3, 0);

    void Start() {
        GameManager.instance.OnStartTurn += StartTurnHandler;
    }

    void Update() {
        if (moving) Move(Time.deltaTime);
    }

    [ClientRpc]
    void StartTurnHandler(object userConnection, int characterId) {
        Debug.Log("Change Beam of Light");
        CharacterController character = CharacterManager.instance.Get(characterId);
        transform.SetParent(character.transform);
        originPosition = Vector3.Scale(transform.position, mask) + offset;
        targetPosition = Vector3.Scale(character.transform.position, mask) + offset;
        moveTime = Vector3.Distance(originPosition, targetPosition) / 4;
        time = 0;
        moving = true;
    }

    void Move(float t) {
        time += t;

        if (time / moveTime > 1) moving = false;

        transform.position = Vector3.Lerp(originPosition, targetPosition, time / moveTime);
    }
}