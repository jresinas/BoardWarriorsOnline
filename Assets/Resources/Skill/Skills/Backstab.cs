using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backstab : SkillNormal {
    public override bool Play(CharacterController target) {
        int dices = (AlliesAdjacent(target) > 0) ? 8 : 4;
        int targetArmor = target.GetArmor();
        int damage = GUIManager.instance.RollDices(dices, targetArmor);
        return damage > 0;
    }

    public override bool TargetAllies() { return false; }
    public override bool TargetEnemies() { return true; }
    public override bool TargetSelf() { return false; }

    int AlliesAdjacent(CharacterController target) {
        int alliesAdjacent = 0;
        CharacterController caster = GetComponent<CharacterController>();
        List<int> allies = CharacterManager.instance.GetPlayerCharacters(caster.GetPlayer());
        Vector2Int position = target.GetPosition();
        Vector2Int[] adjacentVectors = { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        foreach (Vector2Int adjacentVector in adjacentVectors) {
            TileController tileAdjacent = BoardManager.instance.GetTile(position + adjacentVector);
            int characterId = tileAdjacent.GetCharacter();
            if (characterId > 0 && allies.Contains(characterId)) alliesAdjacent++;
        }

        return alliesAdjacent;
    }
}
