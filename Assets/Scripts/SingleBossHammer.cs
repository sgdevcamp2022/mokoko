using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleBossHammer : MonoBehaviour
{
    public int nextMove = -1;
    public float speed = 1.0f;
    public float weight = 1.0f;
    public float damage = 10.0f;
    
    public Image healthBar;
    public GameObject omen;
    public GameObject rock;

    bool isAttack = false;
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
        nextMove = Random.Range(-1, 2);
        int isAttack = Random.Range(0, 10);

        if(nextMove == 0) {
            if(isAttack == 0) {
                StartCoroutine(Attack_1());
                IEnumerator Attack_1() {
                    CancelInvoke("GetDirection");
                    anim.SetInteger("isRun", 0);

                    yield return new WaitForSeconds(3.0f);
                    anim.SetBool("isAttack", true);

                    yield return new WaitForSeconds(4.0f);
                    anim.SetBool("isAttack", false);
                    nextMove = Random.Range(-1, 2);
                    Invoke("GetDirection", 1);
                }
                return;

            } else if(isAttack == 1) {
                StartCoroutine(Attack_2());
                IEnumerator Attack_2() {
                    CancelInvoke("GetDirection");
                    anim.SetBool("isCharge", true);
                    anim.SetInteger("isRun", 0);

                    yield return new WaitForSeconds(5.0f);

                    Vector3[] spawnPos = new Vector3[10];
                    for(int i = 0; i < spawnPos.Length; i++) {
                        Vector3 pos = gameObject.transform.position;
                        spawnPos[i] = new Vector3(Random.Range(pos.x - 10f, pos.x + 10f), pos.y + 13.0f, 0.0f);
                    }

                    foreach(Vector3 pos in spawnPos) {
                        GameObject omens = (GameObject)Instantiate(omen, new Vector3(pos.x, 0.0f, pos.z), Quaternion.identity);
                        yield return new WaitForSeconds(0.2f);
                        Destroy(omens);
                    }

                    yield return new WaitForSeconds(2.0f);

                    foreach(Vector3 pos in spawnPos) {
                        GameObject spawnedRocks = (GameObject)Instantiate(rock, pos, Quaternion.identity);
                        yield return new WaitForSeconds(0.2f);
                    }

                    yield return new WaitForSeconds(4.0f);
                    anim.SetBool("isCharge", false);
                    nextMove = Random.Range(-1, 2);
                    Invoke("GetDirection", 1);
                }
                return;
            }
        }

        anim.SetInteger("isRun", nextMove);
        
        if (nextMove != 0) {
            spriteRenderer.flipX = nextMove == -1;
            anim.SetBool("isFlip", (nextMove == -1));
        }

        float nextThinkTime = Random.Range(1f, 3f);
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

    void CanDamagedChecker(bool flag) {
        canDamaged = flag;
    }
}