using System;
using UnityEngine;

[Serializable]
public class SkillDataObject {
    public Sprite icon;
    public string name;
    public string text;
    public int cost;
    public int range;
}

[CreateAssetMenu(fileName = "CharacterData", menuName = "CharacterData/New CharacterData", order = 51)]
public class CharacterDataObject : ScriptableObject, System.IComparable<CharacterDataObject> {
    public Sprite avatar;
    public string name;
    public string surname;
    public int health;
    public int armor;
    public int movement;
    public SkillDataObject[] skills;


    public int CompareTo(CharacterDataObject other) {
        return string.Compare(name, other.name);
    }
}