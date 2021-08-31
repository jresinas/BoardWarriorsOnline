using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        CharacterController target = GetCharacter(destiny);
        List<int> observers = new List<int>();
        ShoveInfo shoveInfo = Shove(target, self.GetPosition());
        if (shoveInfo.characterCollisionId >= 0) observers.Add(shoveInfo.characterCollisionId);
        string shoveSerialized = JsonUtility.ToJson(shoveInfo);

        return new SkillResult(new int[] { target.id }, true, observers: observers.ToArray(), data: shoveSerialized);
    }

    //public override bool TargetAllies() { return false; }
    //public override bool TargetEnemies() { return true; }
    //public override bool TargetSelf() { return false; }
}
