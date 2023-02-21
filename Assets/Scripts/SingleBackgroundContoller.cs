using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBackgroundContoller : MonoBehaviour
{
    public Transform cameraPos;
    
    public Transform[] backgroundElements;
    public float[] speeds;

    void Update()
    {
        for(int i = 0; i < backgroundElements.Length; i++) {
            Vector3 dir = cameraPos.position - backgroundElements[i].position;
            Vector3 moveVector = new Vector3(dir.x * speeds[i] * Time.deltaTime, dir.y * speeds[i] * Time.deltaTime, 0.0f);
            backgroundElements[i].Translate(moveVector);
        }
    }
}
