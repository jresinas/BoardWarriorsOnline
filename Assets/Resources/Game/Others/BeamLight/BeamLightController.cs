using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BeamLightController : NetworkBehaviour {
    CharacterController currentCharacter = null;
    CharacterController nextCharacter = null;

    void Start() {
        GameManager.instance.OnStartTurn += StartTurnHandler;
    }

    void LateUpdate() {
        if (currentCharacter != null) transform.position = currentCharacter.transform.position + Vector3.up * Const.BEAMLIGHT_OFFSET;
    }

    [ClientRpc]
    void StartTurnHandler(object userConnection, int characterId, bool canSkip) {
        if (currentCharacter == null || characterId != currentCharacter.GetId()) {
            currentCharacter = null;
            nextCharacter = CharacterManager.instance.Get(characterId);
            Vector3 originPosition = transform.position;
            Vector3 targetPosition = nextCharacter.transform.position + Vector3.up * Const.BEAMLIGHT_OFFSET;
            float moveTime = Vector3.Distance(originPosition, targetPosition) / 4;
            StartCoroutine(Move(originPosition, targetPosition, moveTime));
        }
    }

    IEnumerator Move(Vector3 origin, Vector3 destiny, float moveTime) {
        for (float f = 0; f <= moveTime; f += Time.deltaTime) {
            transform.position = Vector3.Lerp(origin, destiny, f / moveTime);
            yield return null;
        }
        currentCharacter = nextCharacter;
        nextCharacter = null;
    }
}