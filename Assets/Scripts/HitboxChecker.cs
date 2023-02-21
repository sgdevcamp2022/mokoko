using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitboxChecker : MonoBehaviour
{
    public bool isBoss = false;
    public float maxHP;
    public Image healthBar;
    public GameObject parentGameObject;

    float HP;
    bool canDamaged;
    
    void Awake() {
        healthBar.fillAmount = 100f;
        HP = maxHP;
    }

    public void cangetDamaged(bool flag) {
        canDamaged = flag;
    }

    public void getDamaged(float damage) {
        if(canDamaged) {
            HP -= damage;
            healthBar.fillAmount = HP / maxHP;
            if(HP <= 0) {
                if(!isBoss) {
                    Die();
                } else {
                    parentGameObject.layer = 8;
                    gameObject.GetComponent<SingleSceneChanger>().SceneEnd();
                }
            }
        }
    }
    
    public float GetHP() {
        return HP;
    }
    
    void Die() {
        parentGameObject.layer = 8;
        parentGameObject.GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(DeActive(5.0f));
        IEnumerator DeActive(float delayTime) {
            yield return new WaitForSeconds(delayTime);
            parentGameObject.SetActive(false);
        }
    }
}