using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class TileController : NetworkBehaviour, IPointerClickHandler {
    [SyncVar]
    public Vector2Int position;

    public void SetPosition(Vector2Int position) {
        this.position = position;
    }

    public void OnPointerClick(PointerEventData eventData) {
        BoardManager.instance.ClickTile(position);
    }
}
