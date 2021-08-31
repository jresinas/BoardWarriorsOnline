using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backstab : SkillNormal {
    public override SkillResult Play(Vector2Int destiny) {
        CharacterController target = GetCharacter(destiny);
        int dices = (AlliesAdjacent(target) > 0) ? 8 : 4;
        int targetArmor = target.GetArmor();
        int damage = RollDices(dices, targetArmor);
        return new SkillResult(new int[] { target.id }, damage > 0);
    }

    //public override bool TargetAllies() { return false; }
    //public override bool TargetEnemies() { return true; }
    //public override bool TargetSelf() { return false; }

    int AlliesAdjacent(CharacterController target) {
        int alliesAdjacent = 0;
        List<int> allies = CharacterManager.instance.GetPlayerCharacters(self.GetPlayer());
        Vector2Int position = target.GetPosition();
        Vector2Int[] adjacentVectors = { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        foreach (Vector2Int adjacentVector in adjacentVectors) {
            int characterId = CharacterManager.instance.GetId(position + adjacentVector);
            if (characterId > 0 && allies.Contains(characterId)) alliesAdjacent++;
        }

        return alliesAdjacent;
    }
}
