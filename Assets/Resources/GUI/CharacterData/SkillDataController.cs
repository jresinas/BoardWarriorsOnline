using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SkillDataController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [SerializeField] int index;

    [SerializeField] Image iconWrap;
    [SerializeField] Image icon;
    [SerializeField] Image textWrap;
    [SerializeField] TextMeshProUGUI text;

    bool selected = false;
    Skill selectedCharacterSkill;

    bool IsEnabled() {
        return selectedCharacterSkill != null && selectedCharacterSkill.IsVisible();
    }

    public void SetSkill(Skill skill) {
        selectedCharacterSkill = skill;
    }

    /// <summary>
    /// Show skill data
    /// </summary>
    /// <param name="skill">Skill to show</param>
    /// <param name="enabled">If false, skill shows as disabled</param>
    public void ShowSkill(Skill skill, bool isSelectedCharacter = false) {
        icon.sprite = skill.GetIcon();
        text.text = skill.GetText();
        if (isSelectedCharacter && IsEnabled()) {
            if (selected) SelectedStyle();
            else EnableStyle();
        } else DisableStyle();
    }

    /// <summary>
    /// Show skill as selected
    /// </summary>
    public void SelectSkill() {
        if (IsEnabled()) {
            selected = true;
            SelectedStyle();
        }
    }

    /// <summary>
    /// Show skill as unselected
    /// </summary>
    public void UnselectSkill() {
        if (IsEnabled()) {
            selected = false;
            EnableStyle();
        }
    }

    #region Events
    // Events
    public void OnPointerEnter(PointerEventData eventData) {
        if (!selected && IsEnabled()) HoverStyle();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!selected && IsEnabled()) EnableStyle();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (IsEnabled()) GUIManager.instance.ClickSkill(index);
    }
    #endregion

    #region Styles
    // Styles
    Color COLOR_ENABLE = new Color32(120, 120, 120, 100);
    Color COLOR_HOVER = new Color32(255, 255, 255, 100);
    Color COLOR_SELECT = new Color32(200, 40, 40, 100);
    Color COLOR_DISABLE = new Color32(0, 0, 0, 100);

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

    void HoverStyle() {
        iconWrap.color = Color.white;
        icon.color = Color.white;
        textWrap.color = COLOR_HOVER;
        text.color = Color.black;
    }

    void SelectedStyle() {
        iconWrap.color = Color.white;
        icon.color = Color.white;
        textWrap.color = COLOR_SELECT;
        text.color = Color.white;
    }
    #endregion
}
