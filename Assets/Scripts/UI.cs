using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public GameObject[] icons;

    Image[] coolTimeSprites;
    public float[] coolTimers = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    public float[] fillAmounts = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    float minus;

    public void ActCoolTime(float coolTime) {
        fillAmounts[0] = 1.0f;
        coolTimers[0] = coolTime;
        minus = coolTime;
    }

    void Start() {
        icons = GameObject.FindGameObjectsWithTag("Icon");
        coolTimeSprites = new Image[icons.Length];

        for(int i = 0; i < icons.Length; i++) {
            GameObject canvas = icons[i].transform.Find("Canvas").gameObject;
            GameObject Image = canvas.transform.Find("Image").gameObject;
            coolTimeSprites[i] = Image.GetComponent<Image>();
        }
    }

    void Update() {
        for(int i = 0; i < coolTimers.Length; i++) {
            if (coolTimers[i] > 0.0f) {
                fillAmounts[i] = fillAmounts[i] - (Time.deltaTime / minus);
                coolTimers[i] -= Time.deltaTime;
                coolTimeSprites[i].fillAmount = fillAmounts[i];
            } else {
                coolTimers[i] = 0.0f;
                fillAmounts[i] = 0.0f;
                minus = 0.0f;
            }
        }
    }
}
