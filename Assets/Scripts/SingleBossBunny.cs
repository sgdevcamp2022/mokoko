using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBossBunny : MonoBehaviour
{
    public int nextMove = 0;
    public float speed;
    public float weight;
    public float damage;

    public GameObject rock;

    bool isAttack = false;
    float currentSpeed;
    bool canDamaged = false;

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
        Debug.DrawRay(frontVec, Vector3.right * nextMove, new Color(0, 0, 1));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.right * nextMove, 2, LayerMask.GetMask("Platform"));
        if (rayHit.collider != null) {
            if(!isAttack) {
                Turn();
            }
        }
    }

    void GetDirection() {
        int isAttack = Random.Range(0, 12);
        if(isAttack < 1) {
            Attack_1();
            return;
        } else if(isAttack < 2) {
            Attack_2();
            return;
        }

        nextMove = Random.Range(-1, 2);
        int idleIdx = Random.Range(0, 2);

        anim.SetInteger("speed", nextMove);
        anim.SetInteger("index", idleIdx);

        if (nextMove != 0) {
            spriteRenderer.flipX = nextMove == 1;
        }

        float nextThinkTime = Random.Range(1f, 3f);
        Invoke("GetDirection", nextThinkTime);
    }

    void Turn() {
        nextMove = -nextMove;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("GetDirection", 1);
    }

    void DeActive() {
        gameObject.SetActive(false);
    }

    void Die() {
        gameObject.layer = 8;
        spriteRenderer.color = Color.red;
        Invoke("DeActive", 5.0f);
    }

    void Attack_1() {
        CancelInvoke("GetDirection");
        isAttack = true;
        currentSpeed = speed;
        anim.SetInteger("speed", 0);
        speed = 0.0f;
        anim.SetTrigger("attack_1");

        Invoke("StartAttack_1", 5.0f);
        
        Invoke("EndAttack_1", 15.0f);
    }

    void StartAttack_1() {
        nextMove = (Random.Range(0, 2) > 0) ? -1 : 1;
        anim.SetInteger("speed", nextMove);
        if (nextMove != 0) {
            spriteRenderer.flipX = nextMove == 1;
        }

        speed = 10.0f;
    }

    void EndAttack_1() {
        speed = currentSpeed;
        isAttack = false;
        Invoke("GetDirection", 1);
    }

    public float GetDamage() {
        return damage;
    }

    void Attack_2() {
        CancelInvoke("GetDirection");
        isAttack = true;
        nextMove = 0;
        anim.SetInteger("speed", 0);
        anim.SetInteger("index", -1);
        anim.SetTrigger("attack_2");

        StartCoroutine(StartAttack_2(5.0f));
        IEnumerator StartAttack_2(float delayTime) {
            yield return new WaitForSeconds(delayTime);
            anim.SetTrigger("attack_2_isAttack");

            for(int i = 0; i < 30; i++) {
                GameObject spawnedRocks = (GameObject)Instantiate(rock, gameObject.transform.position, Quaternion.identity);
                spawnedRocks.GetComponent<ObjectRock>().AddForce(Random.Range(-3.0f, 3.0f), Random.Range(10.0f, 20.0f));
                yield return new WaitForSeconds(0.2f);
            }
        }

        Invoke("EndAttack_2", 11.0f);
    }

    void EndAttack_2() {
        isAttack = false;
        anim.SetInteger("index", (int)Random.Range(0,2));
        Invoke("GetDirection", 1);
    }

    void CanDamagedChecker(bool flag) {
        canDamaged = flag;
    }
}
