using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTarget : MonoBehaviour
{
    public float objectSpeed;
    public float duration = 10.0f;

    public GameObject vanilaFX;

    Transform playerTr;

    public void init(Transform target) {
        playerTr = target;
    }

    void Awake() {
        StartCoroutine(Destroyer(duration));
        StartCoroutine(HitEffect());
    }

    void Update() {
        Vector3 dir = playerTr.position - this.transform.position;
        Vector3 moveVector = new Vector3(dir.x * objectSpeed * Time.deltaTime, dir.y * objectSpeed * Time.deltaTime, 0.0f);
        this.transform.Translate(moveVector);
    }

    IEnumerator Destroyer(float delayTime) {
        yield return new WaitForSeconds(delayTime);

        foreach(GameObject FXs in GameObject.FindGameObjectsWithTag("FX")) {
            Destroy(FXs);
        }
        Destroy(gameObject);
    }

    IEnumerator HitEffect() {
        while(true) {
            StartCoroutine(FX());
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    IEnumerator FX() {
        float randomX = Random.Range(this.transform.position.x - 0.5f, this.transform.position.x + 0.5f);
        float randomY = Random.Range(this.transform.position.y - 0.5f, this.transform.position.y + 0.5f);
        GameObject FX = (GameObject)Instantiate(vanilaFX, new Vector3(randomX, randomY, 0f), Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        Destroy(FX);
    }
}