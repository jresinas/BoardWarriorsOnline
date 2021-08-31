using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillNormal : Skill {
    public override bool IsVisible() {
        return base.IsVisible() && ClientManager.instance.IsAvailableSkill();
    }
}
