using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleEnemy : MonoBehaviour
{
    public int nextMove = -1;
    public float speed = 1.0f;
    public float weight = 1.0f;
    public float damage = 10.0f;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        GetDirection();
    }

    void FixedUpdate() {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 0, 1));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null) {
            Turn();
        }
    }

    void GetDirection() {
        nextMove = Random.Range(-1, 2);

        anim.SetInteger("speed", nextMove);
        
        if (nextMove != 0) {
            spriteRenderer.flipX = nextMove == -1;
        }

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("GetDirection", nextThinkTime);
    }

    void Turn() {
        nextMove = -nextMove;
        spriteRenderer.flipX = nextMove == -1;

        CancelInvoke();
        Invoke("GetDirection", 1);
    }

    void DeActive() {
        gameObject.SetActive(false);
    }

    public float GetDamage() {
        return damage;
    }
}