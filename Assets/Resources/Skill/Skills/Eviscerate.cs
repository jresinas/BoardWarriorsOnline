using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eviscerate : Skill {
    public override bool Play(CharacterController target) {
        int targetArmor = target.GetArmor();
        int damage = GUIManager.instance.RollDices(2, targetArmor + 1);
        return damage > 0;
    }

    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return true; }
    public override bool TargetSelf() { return false; }
}
