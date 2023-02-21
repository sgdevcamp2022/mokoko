using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRock : MonoBehaviour
{
    Rigidbody2D rigidBody;

    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Physics2D.OverlapCircle((Vector2)transform.position, 0.7f, 1 << LayerMask.NameToLayer("Platform"))) {
            Destroy(gameObject);
        }
    }

    public void AddForce(float right, float up) {
        rigidBody.AddForce(Vector2.right * right, ForceMode2D.Impulse);
        rigidBody.AddForce(Vector2.up * up, ForceMode2D.Impulse);
    }
}
