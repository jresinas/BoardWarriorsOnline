using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillDataController : MonoBehaviour {
    Color COLOR_ENABLE = new Color32(120, 120, 120, 100);
    Color COLOR_HOVER = new Color32(255, 255, 255, 100);
    Color COLOR_SELECT = new Color32(200, 40, 40, 100);
    Color COLOR_DISABLE = new Color32(0, 0, 0, 100);

    [SerializeField] Image iconWrap;
    [SerializeField] Image icon;
    [SerializeField] Image textWrap;
    [SerializeField] TextMeshProUGUI text;

    public void ShowSkill(Skill skill, bool enable) {
        icon.sprite = skill.GetIcon();
        text.text = skill.GetText();

        if (enable) EnableStyle();
        else DisableStyle();
    }

    void EnableStyle() {
        iconWrap.color = Color.white;
        icon.color = Color.white;
        textWrap.color = COLOR_ENABLE;
        text.color = Color.white;
    }

    void DisableStyle() {
        iconWrap.color = Color.grey;
        icon.color = Color.grey;
        textWrap.color = COLOR_DISABLE;
        text.color = Color.grey;
    }
}
