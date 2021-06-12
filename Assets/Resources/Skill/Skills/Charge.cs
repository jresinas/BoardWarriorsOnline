using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : Skill {
    public override bool Play(CharacterController target) {
        return true;
    }

    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return true; }
    public override bool TargetSelf() { return false; }
}
