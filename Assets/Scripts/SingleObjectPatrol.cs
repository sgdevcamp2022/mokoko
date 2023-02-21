using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleObjectPatrol : MonoBehaviour
{
    public Vector2 startPoint;
    public Vector2 endPoint;
    
    Vector2 destination;

    void Awake() {
        if(startPoint.x < endPoint.x || startPoint.y < endPoint.y) {
            Vector2 tmp = startPoint;
            startPoint = endPoint;
            endPoint = tmp;
        }
        destination = startPoint;
    }

    void Update()
    {
        if(startPoint != endPoint) {
            if(startPoint.x - 0.1f < transform.position.x && startPoint.y - 0.1f < transform.position.y) {
                destination = endPoint;
            } else if (endPoint.x + 0.1f > transform.position.x && endPoint.y + 0.1f > transform.position.y) {
                destination = startPoint;
            }
            
            transform.position = Vector2.Lerp(new Vector2(transform.position.x, transform.position.y), destination, Time.deltaTime);
        }
    }
}