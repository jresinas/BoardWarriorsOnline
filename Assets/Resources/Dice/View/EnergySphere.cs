using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnergySphere : MonoBehaviour {
    [SerializeField] Material material;
    [SerializeField] TextMeshProUGUI amountTMP;
    Color color;
    int amount;

    void OnEnable() {
        // Set default values
        amount = 0;
        transform.localScale = new Vector3(Const.DICE_SPHERE_BASE_SIZE, Const.DICE_SPHERE_BASE_SIZE, Const.DICE_SPHERE_BASE_SIZE);
    }

    void Update() {
        // Rotate sphere
        transform.Rotate(-0.5f, -0.5f, -0.5f);
        // If alpha is greater than base, decrease it
        color = material.GetColor("_TintColor");
        if (color.a > Const.DICE_SPHERE_BASE_ALPHA) {
            color.a += Const.DICE_SPHERE_INC_ALPHA;
            material.SetColor("_TintColor", color);
        }
    }

    public void Increase() {
        // Increase sphere intensity (alpha)
        color = material.GetColor("_TintColor");
        color.a = Const.DICE_SPHERE_MAX_ALPHA;
        material.SetColor("_TintColor", color);
        // Increse sphere size
        float newSize = Mathf.Min(Const.DICE_SPHERE_BASE_SIZE + (Const.DICE_SPHERE_INC_SIZE * amount), Const.DICE_SPHERE_MAX_SIZE);
        transform.localScale = new Vector3(newSize, newSize, newSize);

        amount++;
        if (amount > 0) amountTMP.text = amount.ToString();
    }
}
