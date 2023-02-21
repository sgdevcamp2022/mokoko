using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneLogo : MonoBehaviour
{
    public float startYPoint;
    public float endYPoint;
    
    float destination;

    void Awake() {
        destination = startYPoint;
    }

    void Update()
    {
        if(startYPoint < transform.position.y + 0.1f) {
            destination = endYPoint;
        } else if (endYPoint > transform.position.y - 0.1f) {
            destination = startYPoint;
        }
        
        transform.position = Vector2.Lerp(transform.position, new Vector2(0, destination), Time.deltaTime);
    }
}
