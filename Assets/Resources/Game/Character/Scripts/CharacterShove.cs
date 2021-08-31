using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Skill data class to retrieve info about shove
/// </summary>
public class ShoveInfo {
    public Vector2Int shovedOrigin;
    public Vector2Int shovedDestiny;
    public Vector2Int shover;
    public int characterCollisionId;

    public ShoveInfo(Vector2Int shovedOrigin, Vector2Int shovedDestiny, Vector2Int shover, int characterCollisionId) {
        this.shovedOrigin = shovedOrigin;
        this.shovedDestiny = shovedDestiny;
        this.characterCollisionId = characterCollisionId;
        this.shover = shover;
    }
}

public class CharacterShove : MonoBehaviour { 
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;

    /// <summary>
    /// Initialize shove effect related to data
    /// </summary>
    /// <param name="data">ShoveInfo data serialized as JSON</param>
    public void StartShove(string data) {
        ShoveInfo shoveInfo = JsonUtility.FromJson<ShoveInfo>(data);

        Vector2Int shoverTile = shoveInfo.shover;
        Vector2Int prevDestinyTile = BoardUtils.GetShoveDestiny(shoveInfo.shovedOrigin, shoverTile);

        Vector3 currentPosition = transform.position;
        Vector3 prevDestinyPosition = BoardManager.instance.GetTile(prevDestinyTile).transform.position + Vector3.up * Const.CHAR_OFFSET;
        Vector3 destinyPosition;

        if (prevDestinyTile.x < 0 || prevDestinyTile.y < 0 || prevDestinyTile.x >= Const.BOARD_COLS || prevDestinyTile.y >= Const.BOARD_ROWS) {
            // Shoved character collide aginst board limits
            destinyPosition = currentPosition + ((prevDestinyPosition - currentPosition) / 2);
        } else {
            if (shoveInfo.characterCollisionId < 0) {
                // Shoved character not collide
                destinyPosition = prevDestinyPosition;
            } else {
                // Shoved character collide against characterCollisionId
                destinyPosition = currentPosition + ((prevDestinyPosition - currentPosition) * 3 / 4);
            }
        }

        Vector3 shoverPosition = BoardManager.instance.GetTile(shoverTile).transform.position + Vector3.up * Const.CHAR_OFFSET;
        transform.LookAt(shoverPosition);
        anim.SetBool("Shove", true);
        StartCoroutine(Shove(currentPosition, destinyPosition, shoveInfo.characterCollisionId));
    }

    IEnumerator Shove(Vector3 originPosition, Vector3 targetPosition, int characterCollisionId) {
        for (float f = 0; f <= Const.SHOVE_ANIM_TIME; f += Time.deltaTime) {
            transform.position = Vector3.Lerp(originPosition, targetPosition, f / Const.SHOVE_ANIM_TIME);
            yield return null;
        }
        EndShove(characterCollisionId);
    }

    void EndShove(int characterCollisionId) {
        CharacterController collisionChar = CharacterManager.instance.Get(characterCollisionId);
        if (collisionChar != null) {
            self.ReceiveImpact("Damage");
            collisionChar.ReceiveImpact("Damage");
        } else {
            anim.SetBool("Shove", false);
        }
    }
}
