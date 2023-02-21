using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPigeon : MonoBehaviour
{
    public float speed;

    Transform trform;
    Animator animator;
    Vector2 target;


    bool movable = false;

    void Awake() {
        trform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        target = new Vector2(trform.position.x + 13.0f, trform.position.y);
    }

    void Update() {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("PigeonBlow") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
            animator.SetBool("actor", true);
            movable = true;
        }
        if (movable) {
            trform.position = Vector2.MoveTowards(trform.position, target, speed * Time.deltaTime);
        }
        if(trform.position.x >= target.x) {
            gameObject.tag = "Untagged";
        }
        if(trform.position.x == target.x) {
            animator.SetBool("actor", false);
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("PigeonBlow") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
                Destroyer();
            }
        }
    }

    void Destroyer() => Destroy(gameObject);
}
