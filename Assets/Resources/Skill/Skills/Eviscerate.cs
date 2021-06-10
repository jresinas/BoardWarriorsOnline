using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eviscerate : Skill {
    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return true; }
    public override bool TargetSelf() { return false; }
}
