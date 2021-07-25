using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickReload : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        return new SkillResult(new int[] { self.id }, true);
    }
}
