using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : SkillResponse {
    public override bool Play(CharacterController target) {
        return true;
    }

    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return false; }
    public override bool TargetSelf() { return true; }
}
