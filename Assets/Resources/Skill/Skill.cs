using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to retrieve skill results execution
/// </summary>
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
    // Skill can target caster's enemies
    [SerializeField] bool targetEnemies;
    // Skill can target caster's allies
    [SerializeField] bool targetAllies;
    // Skill can target to own caster
    [SerializeField] bool targetSelf;
    // Skill target must be in line of sight from the caster
    [SerializeField] bool lineOfSight;
    // Effects and projectiles attached to the skill
    [SerializeField] protected SkillEffect[] effects;

    protected CharacterController self;

    public void Start() {
        self = GetComponent<CharacterController>();
    }

    #region Get&Set
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
    #endregion

    /// <summary>
    /// Execute skill in server
    /// </summary>
    /// <param name="destiny">Skill target position</param>
    /// <returns>Result after skill execution</returns>
    public abstract SkillResult Play(Vector2Int destiny);

    /// <summary>
    /// Determine if skill is usable in current context
    /// </summary>
    public virtual bool IsVisible() {
        return self.GetEnergy() >= energy;
    }

    /// <summary>
    /// Determine if skill targets a character
    /// </summary>
    public bool TargetCharacter() {
        return targetEnemies || targetAllies || targetSelf;
    }

    /// <summary>
    /// Determine if skill targets a tile (not a character)
    /// </summary>
    public bool TargetTile() {
        return !TargetCharacter();
    }

    /// <summary>
    /// Determine if specified position is a valid target
    /// </summary>
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

    /// <summary>
    /// Get list of targeteable character ids for this skill casted by specified character
    /// </summary>
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

    /// <summary>
    /// Starts specified effect during skill using animation
    /// </summary>
    /// <param name="index">Skill effect index</param>
    public virtual void AnimationEffect(int index) {
        if (effects.Length > index) {
            SkillEffect effect = effects[index];
            if (effect != null) {
                GameObject effectInstance = Instantiate(effect.prefab, effect.position.position, effect.rotation.rotation);
                Destroy(effectInstance, effect.time);
                ProjectileController pc = effectInstance.GetComponentInChildren<ProjectileController>();
                if (pc != null) pc.StartProjectile(self);
                AE_SetMeshToEffect smte = effectInstance.GetComponentInChildren<AE_SetMeshToEffect>();
                if (smte != null) smte.Mesh = effect.position.gameObject;
            }
        }
    }

    #region ProtectedMethods
    /// <summary>
    /// Simulate a dice roll
    /// </summary>
    /// <param name="dices">Number of dices</param>
    /// <param name="targetArmor">Target armor (vlaue required to success)</param>
    /// <returns>Number of successful dices</returns>
    protected int RollDices(int dices, int targetArmor) {
        return DiceManager.instance.RollDices(dices, targetArmor);
    }

    /// <summary>
    /// Get CaracterController at specified position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    protected CharacterController GetCharacter(Vector2Int position) {
        int targetId = CharacterManager.instance.GetId(position);
        if (targetId >= 0) return CharacterManager.instance.Get(targetId);
        else return null;
    }

    /// <summary>
    /// Shove specified character from specified position
    /// </summary>
    /// <param name="shoved">Character who receive shove</param>
    /// <param name="shoverTile">Position from which shove</param>
    /// <returns>Info about shove result</returns>
    protected ShoveInfo Shove(CharacterController shoved, Vector2Int shoverTile) {
        List<CharacterController> shoveDamage = new List<CharacterController>();

        int characterCollisionId = -1;
        Vector2Int origin = shoved.GetPosition();
        Vector2Int destiny;
        Vector2Int prevDestiny = BoardUtils.GetShoveDestiny(origin, shoverTile);
        if (prevDestiny.x < 0 || prevDestiny.y < 0 || prevDestiny.x >= Const.BOARD_COLS || prevDestiny.y >= Const.BOARD_ROWS) {
            // Shoved character collide aginst board limits
            destiny = origin;
            shoveDamage.Add(shoved);
        } else {
            characterCollisionId = CharacterManager.instance.GetId(prevDestiny);
            if (characterCollisionId < 0) {
                // Shoved character not collide
                shoved.SetPosition(prevDestiny);
                destiny = prevDestiny;
            } else {
                // Shoved character collide against characterCollisionId
                destiny = origin;
                CharacterController characterCollision = CharacterManager.instance.Get(characterCollisionId);
                shoveDamage.Add(shoved);
                shoveDamage.Add(characterCollision);
            }
        }
        StartCoroutine(ShoveDamage(shoveDamage));

        return new ShoveInfo(origin, destiny, shoverTile, characterCollisionId);
    }
    #endregion

    #region PrivateMethods
    // Apply effect of collision after shove some time later to sync with animation
    IEnumerator ShoveDamage(List<CharacterController> characters) {
        yield return new WaitForSeconds(Const.SHOVE_COLLISION_TIME);
        foreach (CharacterController character in characters) character.ChangeHealth(-1);
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
    #endregion
}
