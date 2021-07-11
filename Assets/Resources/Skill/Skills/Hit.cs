using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : SkillNormal {
    [SerializeField] int dicesNumber;

    public override SkillResult Play(Vector2Int destiny) {
        CharacterController target = GetTarget(destiny);
        int targetArmor = target.GetArmor();
        int damage = RollDices(dicesNumber, targetArmor+1);
        target.ChangeHealth(-damage);
        return new SkillResult(new int[] { target.id }, damage > 0);
    }

    //public override bool TargetAllies() { return true; }
    //public override bool TargetEnemies() {return true; }
    //public override bool TargetSelf() { return false; }
}
