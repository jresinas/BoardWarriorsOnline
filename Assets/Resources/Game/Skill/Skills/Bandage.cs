using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandage : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        CharacterController target = GetCharacter(destiny);
        target.ChangeHealth(2);
        return new SkillResult(new int[] { target.id }, true);
    }

    //public override bool TargetAllies() { return true; }
    //public override bool TargetEnemies() { return false; }
    //public override bool TargetSelf() { return true; }
}
