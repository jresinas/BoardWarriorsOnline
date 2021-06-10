using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class TileController : NetworkBehaviour, IPointerClickHandler {
    [SyncVar] Vector2Int position;

    public void SetPosition(Vector2Int position) {
        this.position = position;
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public void HideMark() {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = new Color32(203, 202, 248, 255);
    }

    public void ShowMoveMark() {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = new Color32(125, 200, 125, 255);
    }

    public void ShowTargetMark() {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = new Color32(200, 125, 125, 255);
    }

    public int GetCharacter() {
        return CharacterManager.instance.GetId(position);
    }

    public void OnPointerClick(PointerEventData eventData) {
        BoardManager.instance.ClickTile(position);
    }
}
