using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class CharacterData : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, System.IComparable<CharacterData> {
    public CharacterDataObject characterDataObject;
    [SerializeField] Image avatar;
    [SerializeField] TextMeshProUGUI name;
    public Vector3 originalPosition;
    Transform canvas;
    
    
    public CharacterContainer currentContainer;

    

    RectTransform rt;


    void Awake() {
        Canvas canv = GetComponentInParent<Canvas>();
        if (canv != null) canvas = canv.transform;

        rt = GetComponent<RectTransform>();
        rt.position = Vector3.zero;
    }

    public void SetContainer(CharacterContainer content) {
        rt.SetParent(content.container, true);
        currentContainer = content;
    }

    public void Initialize(CharacterDataObject character) {
        characterDataObject = character;
        avatar.sprite = character.avatar;
        name.text = character.name + ", " + character.surname;

        
    }

    public void OnBeginDrag(PointerEventData eventData) {
        originalPosition = rt.localPosition;
        rt.SetParent(canvas, true);
    }

    public void OnDrag(PointerEventData eventData) {
        rt.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (RaycastResult result in results) {
            CharacterContainer container = result.gameObject.GetComponent<CharacterContainer>();
            if (container != null && container != currentContainer) {
                currentContainer.RemoveCharacter(this);
                container.AddCharacter(this);
                return;
            }
        }

        SetContainer(currentContainer);
        rt.localPosition = originalPosition;
    }

    public int CompareTo(CharacterData other) {
        return characterDataObject.CompareTo(other.characterDataObject);
    }
}
