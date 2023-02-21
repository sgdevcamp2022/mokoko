using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSceneEndingManager : MonoBehaviour
{
    public GameObject endingCredit;
    public GameObject[] childs;

    public float delayTime;
    
    void Awake() {
        StartCoroutine(EndingAnimation());
    }
    
    IEnumerator EndingAnimation() {
        yield return new WaitForSeconds(delayTime);
        foreach(GameObject child in childs) {
            SpriteRenderer childSprite = child.GetComponent<SpriteRenderer>();
            while(childSprite.color[3] > 0.0f) {
                childSprite.color -= new Color(0.0f, 0.0f, 0.0f, 0.05f);
                yield return new WaitForSeconds(0.005f);
            }
        }
        endingCredit.SetActive(true);
    }
}