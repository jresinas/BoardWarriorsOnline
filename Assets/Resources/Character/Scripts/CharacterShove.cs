using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShove : MonoBehaviour { 
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;

    public Vector2Int origin;


    public void StartShove(Vector2Int origin) {
        Vector2Int destiny = BoardUtils.GetShoveDestiny(self.position, origin);

        /**/
        CharacterController collisionChar = null;
        if (destiny.x < 0 || destiny.y < 0 || destiny.x >= Const.BOARD_COLS || destiny.y >= Const.BOARD_ROWS) {
            // Choca con limite del tablero
        } else {
            int collisionId = CharacterManager.instance.GetId(destiny);
            if (collisionId < 0) {
                // Se le empuja correctamente
                self.SetPosition(destiny);
                //collisionChar = null;
            } else {
                // choca contra collisionId
                collisionChar = CharacterManager.instance.Get(collisionId);
            }
        }
        if (collisionChar != null) {
            self.ReceiveDamage();
            collisionChar.ReceiveDamage();
        }
        /**/
        
        Vector3 currentPosition = transform.position;
        Vector3 originPosition = BoardManager.instance.GetTile(origin).transform.position + Vector3.up * Const.CHAR_OFFSET;
        Vector3 destinyPosition = BoardManager.instance.GetTile(destiny).transform.position + Vector3.up * Const.CHAR_OFFSET;
        transform.LookAt(originPosition);
        anim.SetBool("Shove", true);
        StartCoroutine(Shove(currentPosition, destinyPosition));
    }

    void EndShove() {
        Debug.Log("End Shove");
        anim.SetBool("Shove", false);
        //self.Idle();
    }

    IEnumerator Shove(Vector3 originPosition, Vector3 targetPosition) {
        for (float f = 0; f <= 0.8f; f += Time.deltaTime) {
            transform.position = Vector3.Lerp(originPosition, targetPosition, f / 0.8f);
            yield return null;
        }
        EndShove();
    }
}
