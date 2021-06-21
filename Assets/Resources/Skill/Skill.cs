using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {
    [SerializeField] string title;
    [SerializeField] Sprite icon;
    [SerializeField] string text;
    [SerializeField] string animation = "Attack";
    [SerializeField] protected int range;
    [SerializeField] protected int energy;

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

    public abstract bool TargetEnemies();

    public abstract bool TargetAllies();

    public abstract bool TargetSelf();

    public abstract bool Play(CharacterController target);

    public virtual bool IsVisible() {
        CharacterController caster = GetComponent<CharacterController>();
        return caster.GetEnergy() >= energy;
    }

    /// <summary>
    /// Get list of targeteable character ids for this skill casted by specified character
    /// </summary>
    /// <param name="caster"></param>
    /// <returns></returns>
    public virtual List<int> GetTargetList(CharacterController caster) {
        List<int> targetIds = new List<int>();
        int alliesPlayer = caster.GetPlayer();
        int enemiesPlayer = ((alliesPlayer + 1) % 2);
        if (TargetAllies()) targetIds.AddRange(CharacterManager.instance.GetPlayerCharacters(alliesPlayer));
        if (TargetEnemies()) targetIds.AddRange(CharacterManager.instance.GetPlayerCharacters(enemiesPlayer));
        if (TargetSelf()) targetIds.Add(caster.GetId());
        else targetIds.Remove(caster.GetId());
        return targetIds;
    }
}
