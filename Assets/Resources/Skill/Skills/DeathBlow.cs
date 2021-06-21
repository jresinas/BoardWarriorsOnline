using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlow : SkillNormal {
    public override bool Play(CharacterController target) {
        int targetArmor = target.GetArmor();
        int damage = GUIManager.instance.RollDices(10, targetArmor + 1);
        return damage > 0;
    }

    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return true; }
    public override bool TargetSelf() { return false; }
}
