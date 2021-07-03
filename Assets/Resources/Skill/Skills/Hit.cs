using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : SkillNormal {
    [SerializeField] int dicesNumber;

    public override bool Play(CharacterController target) {
        int targetArmor = target.GetArmor();
        int damage = RollDices(dicesNumber, targetArmor+1);
        return damage > 0;
    }

    public override bool TargetAllies() { return true; }
    public override bool TargetEnemies() {return true; }
    public override bool TargetSelf() { return false; }
}
