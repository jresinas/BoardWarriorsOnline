using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillResult{
    public bool success;
    public int[] targets;

    public SkillResult(int[] targets, bool success) {
        this.targets = targets;
        this.success = success;
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
    //[SerializeField] GameObject projectile;
    [SerializeField] protected SkillEffect[] effects;

    protected CharacterController self;

    public void Start() {
        self = GetComponent<CharacterController>();
    }

    public int GetRange() {
        return range;
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
                // distance between caster and target is <= skill range and target is targetable by the skill
                return (BoardUtils.Distance(self.GetPosition(), target.GetPosition()) <= GetRange()) &&
                    (targetableIds.Count != 0 && targetableIds.Contains(targetId));
            }
        } else if (TargetTile()) {
            return BoardUtils.Distance(self.GetPosition(), destiny) <= GetRange() && self.GetPosition() != destiny;
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
}
