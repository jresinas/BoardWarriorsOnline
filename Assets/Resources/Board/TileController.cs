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
        mr.material.color = Const.TILE_DEFAULT;
    }

    public void ShowMoveMark() {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = Const.TILE_GREEN;
    }

    public void ShowSkillMark() {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = Const.TILE_RED;
    }

    public void OnPointerClick(PointerEventData eventData) {
        BoardManager.instance.ClickTile(position);
    }
}
