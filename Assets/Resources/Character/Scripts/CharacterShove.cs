using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //public Vector2Int shoverPosition;


    public void StartShove(string data) {
        Debug.Log(data);
        //ShoveInfo shoveInfo = (ShoveInfo)data;
        ShoveInfo shoveInfo = JsonUtility.FromJson<ShoveInfo>(data);

        Vector2Int shoverTile = shoveInfo.shover;
        Vector2Int prevDestinyTile = BoardUtils.GetShoveDestiny(shoveInfo.shovedOrigin, shoverTile);

        Vector3 currentPosition = transform.position;
        Vector3 prevDestinyPosition = BoardManager.instance.GetTile(prevDestinyTile).transform.position + Vector3.up * Const.CHAR_OFFSET;
        Vector3 destinyPosition;

        /**/
        if (prevDestinyTile.x < 0 || prevDestinyTile.y < 0 || prevDestinyTile.x >= Const.BOARD_COLS || prevDestinyTile.y >= Const.BOARD_ROWS) {
            // Choca con limite del tablero
            destinyPosition = currentPosition + ((prevDestinyPosition - currentPosition) / 2);
        } else {
            if (shoveInfo.characterCollisionId < 0) {
                // Se le empuja correctamente
                destinyPosition = prevDestinyPosition;
            } else {
                // choca contra collisionId
                destinyPosition = currentPosition + ((prevDestinyPosition - currentPosition) * 3 / 4);
            }
        }
        /**/

        Vector3 originPosition = BoardManager.instance.GetTile(shoverTile).transform.position + Vector3.up * Const.CHAR_OFFSET;
        transform.LookAt(originPosition);
        anim.SetBool("Shove", true);
        StartCoroutine(Shove(currentPosition, destinyPosition, shoveInfo.characterCollisionId));
    }

    //public void StartShove(Vector2Int shoverTile) {
    //    Vector2Int destinyTile = BoardUtils.GetShoveDestiny(self.position, shoverTile);
    //    Vector3 currentPosition = transform.position;
    //    Vector3 destinyPosition = BoardManager.instance.GetTile(destinyTile).transform.position + Vector3.up * Const.CHAR_OFFSET;

    //    /**/
    //    //CharacterController collisionChar = null;
    //    if (destinyTile.x < 0 || destinyTile.y < 0 || destinyTile.x >= Const.BOARD_COLS || destinyTile.y >= Const.BOARD_ROWS) {
    //        // Choca con limite del tablero
    //        destinyPosition = currentPosition + ((destinyPosition - currentPosition) / 2);
    //    } else {
    //        int collisionId = CharacterManager.instance.GetId(destinyTile);
    //        if (collisionId < 0) {
    //            // Se le empuja correctamente
    //            destinyPosition = BoardManager.instance.GetTile(self.position).transform.position + Vector3.up * Const.CHAR_OFFSET;
    //            Debug.Log(self.position);
    //            Debug.Log(currentPosition);
    //            Debug.Log(destinyPosition);
    //            //self.SetPosition(destiny);
    //            //collisionChar = null;
    //        } else {
    //            // choca contra collisionId
    //            collisionChar = CharacterManager.instance.Get(collisionId);
    //            destinyPosition = currentPosition + ((destinyPosition - currentPosition) * 3 / 4);
    //        }
    //    }
        
    //    /**/
        
    //    Vector3 originPosition = BoardManager.instance.GetTile(shoverTile).transform.position + Vector3.up * Const.CHAR_OFFSET;
    //    transform.LookAt(originPosition);
    //    anim.SetBool("Shove", true);
    //    StartCoroutine(Shove(currentPosition, destinyPosition));
    //}
  

    void EndShove(int characterCollisionId) {
        CharacterController collisionChar = CharacterManager.instance.Get(characterCollisionId);

        Debug.Log("End Shove");
        if (collisionChar != null) {
            self.ReceiveDamage();
            collisionChar.ReceiveDamage();
        } else {
            anim.SetBool("Shove", false);
        }

        collisionChar = null;
        //self.Idle();
    }

    IEnumerator Shove(Vector3 originPosition, Vector3 targetPosition, int characterCollisionId) {
        for (float f = 0; f <= 0.8f; f += Time.deltaTime) {
            transform.position = Vector3.Lerp(originPosition, targetPosition, f / 0.8f);
            yield return null;
        }
        EndShove(characterCollisionId);
    }
}
