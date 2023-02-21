using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHook : MonoBehaviour
{
    public float objectSpeed;
    
    GameObject player;
    int direction;

    Transform trform;
    Vector2 target;

    void Awake() {
        player = GameObject.FindWithTag("Player");
        trform = GetComponent<Transform>();
        direction = trform.position.x < 0.0f ? 1 : -1;

        trform.localScale = new Vector3(direction * trform.localScale.x, trform.localScale.y, trform.localScale.z);
        target = new Vector2(trform.position.x + direction * 30.0f, trform.position.y);
    }

    void Update() {
        trform.position = Vector2.MoveTowards(trform.position, target, objectSpeed * Time.deltaTime);

        if(trform.position.x == target.x) {
            Destroy(gameObject);
        }
    }
    
    // void OnCollisionEnter2D(Collision2D collision) {
    //     player.transform.position = trform.position;
    // }
}
