using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        return new SkillResult(new int[] { self.id }, true);
    }

    //public override bool TargetAllies() { return false; }
    //public override bool TargetEnemies() { return false; }
    //public override bool TargetSelf() { return true; }
}
