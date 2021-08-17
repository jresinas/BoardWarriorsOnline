using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillResult{
    public bool success;
    public int[] targets;
    public int[] observers;
    public string data;

    public SkillResult(int[] targets, bool success, int[] observers = null, string data = null) {
        this.targets = targets;
        this.success = success;
        this.observers = (observers ??= new int[0]);
        this.data = data;
    }
}

[Serializable]
public class SkillEffect {
    public GameObject prefab;
    public Transform position;
    public Transform rotation;
    public float time;
}

public abstract class Skill : MonoBehaviour {
    [SerializeField] string title;
    [SerializeField] Sprite icon;
    [SerializeField] string text;
    [SerializeField] string animation = "Attack";
    [SerializeField] protected int range;
    [SerializeField] protected int energy;
    [SerializeField] bool targetEnemies;
    [SerializeField] bool targetAllies;
    [SerializeField] bool targetSelf;
    [SerializeField] bool lineOfSight;
    //[SerializeField] GameObject projectile;
    [SerializeField] protected SkillEffect[] effects;

    protected CharacterController self;

    public void Start() {
        self = GetComponent<CharacterController>();
    }

    public int GetRange() {
        return range;
    }

    public int GetEnergy() {
        return energy;
    }

    public Sprite GetIcon() {
        return icon;
    }

    public string GetTitle() {
        return title;
    }

    public string GetText() {
        return text;
    }

    public string GetAnimation() {
        return animation;
    }

    public abstract SkillResult Play(Vector2Int destiny);

    public virtual bool IsVisible() {
        return self.GetEnergy() >= energy;
    }

    public bool TargetCharacter() {
        return targetEnemies || targetAllies || targetSelf;
    }

    public bool TargetTile() {
        return !TargetCharacter();
    }

    public virtual bool AllowTarget(Vector2Int destiny) {
        if (TargetCharacter()) {
            int targetId = CharacterManager.instance.GetId(destiny);
            CharacterController target = CharacterManager.instance.Get(targetId);
            if (target != null) {
                List<int> targetableIds = GetTargetableCharacters(self);
                // distance between caster and target is <= skill range and target is targetable by the skill and there is line of sight (if needed)
                // return (BoardUtils.Distance(self.GetPosition(), target.GetPosition()) <= GetRange()) &&
                return (BoardUtils.Reach(self.GetPosition(), target.GetPosition(), GetRange()) &&
                    targetableIds.Count != 0 && targetableIds.Contains(targetId) &&
                    (LineOfSight(self, target) || !lineOfSight));
            }
        } else if (TargetTile()) {
            // return BoardUtils.Distance(self.GetPosition(), destiny) <= GetRange() && self.GetPosition() != destiny;
            return BoardUtils.Reach(self.GetPosition(), destiny, GetRange()) && self.GetPosition() != destiny;
        }
        return false;
    }

    
    bool LineOfSight(CharacterController caster, CharacterController target) {
        Vector3 direction = (target.transform.position + Vector3.up * 0.5f - caster.transform.position + Vector3.up * 0.5f).normalized;
        RaycastHit[] allHits;
        allHits = Physics.RaycastAll(caster.transform.position, direction, 10);
        foreach (var hit in allHits) {
            CharacterController nextCharacter = hit.collider.GetComponentInParent<CharacterController>();
            if (nextCharacter == target) return true;
            if (nextCharacter.GetPlayer() != caster.GetPlayer()) return false;
        }
        return false;
    }
    

    /// <summary>
    /// Get list of targeteable character ids for this skill casted by specified character
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public virtual List<int> GetTargetableCharacters(CharacterController caster) {
        List<int> targetIds = new List<int>();
        int alliesPlayer = caster.GetPlayer();
        int enemiesPlayer = ((alliesPlayer + 1) % 2);
        if (targetAllies) targetIds.AddRange(CharacterManager.instance.GetPlayerCharacters(alliesPlayer));
        if (targetEnemies) targetIds.AddRange(CharacterManager.instance.GetPlayerCharacters(enemiesPlayer));
        if (targetSelf) targetIds.Add(caster.GetId());
        else targetIds.Remove(caster.GetId());
        return targetIds;
    }

    protected int RollDices(int dices, int targetArmor) {
        return DiceManager.instance.RollDices(dices, targetArmor);
    }

    protected CharacterController GetTarget(Vector2Int destiny) {
        int targetId = CharacterManager.instance.GetId(destiny);
        if (targetId >= 0) return CharacterManager.instance.Get(targetId);
        else return null;
    }

    public virtual void AnimationEffect(int number) {
        if (effects.Length > number) {
            SkillEffect effect = effects[number];

            if (effect != null) {
                GameObject effectInstance = Instantiate(effect.prefab, effect.position.position, effect.rotation.rotation);
                //GameObject effectInstance = Instantiate(effect, self.leftHand.position, self.transform.rotation);
                //GameObject effectInstance = Instantiate(effect, self.leftHand.position, self.leftHand.rotation);
                Destroy(effectInstance, effect.time);
                ProjectileController pc = effectInstance.GetComponentInChildren<ProjectileController>();
                if (pc != null) pc.StartProjectile(self);
                AE_SetMeshToEffect smte = effectInstance.GetComponentInChildren<AE_SetMeshToEffect>();
                if (smte != null) smte.Mesh = effect.position.gameObject;
            }
        }
    }

    /*
    protected CharacterController Shove2(CharacterController shoved, Vector2Int shoverTile) {
        CharacterController collisionChar;
        Vector2Int destiny = BoardUtils.GetShoveDestiny(shoved.position, shoverTile);
        if (destiny.x < 0 || destiny.y < 0 || destiny.x >= Const.BOARD_COLS || destiny.y >= Const.BOARD_ROWS) {
            // Choca con limite del tablero
            collisionChar = self;
        } else {
            int collisionId = CharacterManager.instance.GetId(destiny);
            if (collisionId < 0) {
                // Se le empuja correctamente
                shoved.SetPosition(destiny);
                collisionChar = null;
            } else {
                // choca contra collisionId
                collisionChar = CharacterManager.instance.Get(collisionId);
            }
        }

        //target.PrepareShoveAnimation(origin);
        return collisionChar;
    }
    */

    protected ShoveInfo Shove(CharacterController shoved, Vector2Int shoverTile) {
        List<CharacterController> shoveDamage = new List<CharacterController>();

        int characterCollisionId = -1;
        Vector2Int origin = shoved.GetPosition();
        Vector2Int destiny;
        Vector2Int prevDestiny = BoardUtils.GetShoveDestiny(origin, shoverTile);
        if (prevDestiny.x < 0 || prevDestiny.y < 0 || prevDestiny.x >= Const.BOARD_COLS || prevDestiny.y >= Const.BOARD_ROWS) {
            // Choca con limite del tablero
            destiny = origin;
            shoveDamage.Add(shoved);
        } else {
            characterCollisionId = CharacterManager.instance.GetId(prevDestiny);
            if (characterCollisionId < 0) {
                // Se le empuja correctamente
                shoved.SetPosition(prevDestiny);
                destiny = prevDestiny;
            } else {
                // choca contra collisionId
                destiny = origin;
                CharacterController characterCollision = CharacterManager.instance.Get(characterCollisionId);
                shoveDamage.Add(shoved);
                shoveDamage.Add(characterCollision);
            }
        }
        StartCoroutine(ShoveDamage(shoveDamage));

        return new ShoveInfo(origin, destiny, shoverTile, characterCollisionId);
    }

    IEnumerator ShoveDamage(List<CharacterController> characters) {
        yield return new WaitForSeconds(2f);
        foreach (CharacterController character in characters) character.ChangeHealth(-1);
    }
}
