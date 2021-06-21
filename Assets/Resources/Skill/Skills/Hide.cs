using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hide : Skill {
    public override bool Play(CharacterController target) {
        return true;
    }

    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return false; }
    public override bool TargetSelf() { return true; }

    public override bool IsVisible() {
        CharacterController caster = GetComponent<CharacterController>();
        return ClientManager.instance.IsResponding() && caster.GetEnergy() >= energy;
    }

    public override bool IsSkillResponse() {
        return true;
    }
}
