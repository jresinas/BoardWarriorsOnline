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

        //return new SkillResult(new int[] { target.id }, damage > 0);

        /*
                List<int> targets = new List<int>( new int[] { target.id });
                int collisionId = target.Shove(self.position);
                if (collisionId == target.id) target.ChangeHealth(-1);
                else if (collisionId >= 0) {
                    CharacterController collisionCharacter = CharacterManager.instance.Get(collisionId);
                    if (collisionCharacter != null) {
                        collisionCharacter.ChangeHealth(-1);
                        targets.Add(collisionCharacter.id);
                    }
                }

                return new SkillResult(targets.ToArray(), damage > 0);
        */

        /*
        List<int> observers = new List<int>();
        List<CharacterController> shoveDamage = new List<CharacterController>();
        CharacterController collisionCharacter = Shove2(target, self.position);
        if (collisionCharacter != null) {
            if (collisionCharacter != target) {
                observers.Add(collisionCharacter.id);
                shoveDamage.Add(target);
                shoveDamage.Add(collisionCharacter);
            } else {
                shoveDamage.Add(target);
            }
            StartCoroutine(ShoveDamage(shoveDamage));
        }
        */

        
        if (target.GetHealth() > 0) {
            List<int> observers = new List<int>();
            ShoveInfo shoveInfo = Shove(target, self.GetPosition());
            if (shoveInfo.characterCollisionId >= 0) observers.Add(shoveInfo.characterCollisionId);
            string shoveSerialized = JsonUtility.ToJson(shoveInfo);
            return new SkillResult(new int[] { target.id }, damage > 0, observers: observers.ToArray(), data: shoveSerialized);
        } else return new SkillResult(new int[] { target.id }, damage > 0);

    }

    

    //public override SkillResult Play(Vector2Int destiny) {
    //    CharacterController target = GetTarget(destiny);
    //    int targetArmor = target.GetArmor();
    //    int damage = RollDices(dicesNumber, targetArmor + 1);
    //    target.ChangeHealth(-damage);
    //    return new SkillResult(new int[] { target.id }, damage > 0);
    //}

    //public override bool TargetAllies() { return true; }
    //public override bool TargetEnemies() {return true; }
    //public override bool TargetSelf() { return false; }
}
