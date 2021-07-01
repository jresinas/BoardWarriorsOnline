using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : SkillNormal {
    public override bool Play(CharacterController target) {
        target.ChangeHealth(2);
        return true;
    }

    public override bool TargetAllies() { return true; }
    public override bool TargetEnemies() { return false; }
    public override bool TargetSelf() { return true; }
}
