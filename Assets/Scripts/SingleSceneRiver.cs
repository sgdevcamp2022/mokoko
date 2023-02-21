using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSceneRiver : MonoBehaviour
{
    public float speed;

    Transform transform;

    void Awake() {
        transform = GetComponent<Transform>();        
    }

    void Update()
    {
        if(transform.position.x < 254.0f) {
            transform.position += new Vector3(42.0f, 0.0f, 0.0f);
        }

        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.left * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }
}
