using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireArrow : SkillNormal {
    [SerializeField] int dicesNumber;

    public override SkillResult Play(Vector2Int destiny) {
        CharacterController target = GetCharacter(destiny);
        int targetArmor = target.GetArmor();
        int damage = RollDices(dicesNumber, targetArmor+1);
        target.ChangeHealth(-damage);
        return new SkillResult(new int[] { target.id }, damage > 0);
    }
}
