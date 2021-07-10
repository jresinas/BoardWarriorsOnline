using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        return new SkillResult(new int[] { }, true);
    }

    //public override bool TargetAllies() { return false; }
    //public override bool TargetEnemies() { return true; }
    //public override bool TargetSelf() { return false; }
}
