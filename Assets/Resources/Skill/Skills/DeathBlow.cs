using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlow : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        CharacterController target = GetTarget(destiny);
        int targetArmor = target.GetArmor();
        int damage = RollDices(10, targetArmor + 1);
        return new SkillResult(new int[] { target.id }, damage > 0);
    }

    //public override bool TargetAllies() { return false; }
    //public override bool TargetEnemies() { return true; }
    //public override bool TargetSelf() { return false; }
}
