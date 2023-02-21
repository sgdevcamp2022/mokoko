using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBackground : MonoBehaviour
{
    public float speed;
    public int startIndex;
    public int endIndex;
    public Transform[] sprites;

    float viewHeigh;

    void Awake() {
        viewHeigh = Camera.main.orthographicSize * 2;
    }

    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        if(sprites[endIndex]. position.y < viewHeigh*(-1)) {
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeigh;

            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave -1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
