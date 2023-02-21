using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class SinglePlayer : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public GameObject respawnBtn;
    public TMP_Text gemScore;
    
    public TMP_Text nicknameText;
    public Image healthBar;

    public GameObject[] ThrowingWeapons;

    public AudioClip[] jumpSounds;
    public AudioClip[] attackSounds;

    public float maxSpeed;
    public float jumpPower;

    public float health;

    bool movable = true;
    bool isGround;
    int score = 0;
    int gemsInGame;

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audiosource;
    Transform transform;
    GameObject nearestEnemyInfo = null;

    Vector3 curPos;

    class Skills {
        public float skillDamage;
        public float skillCoolTime;
        public bool usable;
        public int skillIdx;
        public string effect;

        public Skills(float Damage, float CoolTime, int idx, string FX = null) {
            this.skillDamage = Damage;
            this.skillCoolTime = CoolTime;
            this.usable = true;
            this.skillIdx = idx;
            this.effect = FX;
        }
    }

    Skills hammerAttack = new Skills(180.0f, 2.0f, 0);
    Skills hammerSwing = new Skills(100.0f, 1.0f, 1);
    Skills hammerThrow = new Skills(150.0f, 5.0f, 2, "SpawnHammer");

    void Awake() {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audiosource = GetComponent<AudioSource>();
        transform = GetComponent<Transform>();

        nicknameText.text = "Mokoko";
        nicknameText.color = Color.white;

        gemsInGame = GameObject.FindGameObjectsWithTag("Item").Length;
        gemScore.text = score.ToString() + " / " + gemsInGame.ToString();
    }

    void Update() {
        if (movable) {
            if (Input.GetButtonUp("Horizontal")) {
                rigidBody.velocity = new Vector2((float)rigidBody.velocity.normalized.x * 0.1f, rigidBody.velocity.y);
            }

            float axis = Input.GetAxisRaw("Horizontal");
            if (Input.GetButton("Horizontal")) {
                FlipX(axis);
                animator.SetBool("isWalking", true);
            } else {
                animator.SetBool("isWalking", false);
            }

            if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping")) {
                JumpRPC();
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                Attack(hammerAttack);
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                Attack(hammerSwing);
            }
            
            if (Input.GetKeyDown(KeyCode.E)) {
                Attack(hammerThrow);
            }
        }
        if (health <= 0) {
            gameObject.layer = 8;
            spriteRenderer.color = Color.red;
        }
    }

    void FixedUpdate() {
        if (movable) {
            float h = Input.GetAxisRaw("Horizontal");
            rigidBody.AddForce(Vector2.right * h, ForceMode2D.Impulse);

            if (rigidBody.velocity.x > maxSpeed) {
                rigidBody.velocity = new Vector2(maxSpeed, rigidBody.velocity.y);
            } else if (rigidBody.velocity.x < -maxSpeed) {
                rigidBody.velocity = new Vector2(-maxSpeed, rigidBody.velocity.y);
            }
        }

        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), 0.07f, 1 << LayerMask.NameToLayer("Platform"));
        animator.SetBool("isJumping", !isGround);

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    void FlipX(float axis) {
        spriteRenderer.flipX = axis == -1;
    }

    void JumpRPC() {
        rigidBody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        audiosource.clip = jumpSounds[UnityEngine.Random.Range(0, jumpSounds.Length)];
        audiosource.Play();
    }

    void AttackAnimation(Skills skill) {
        animator.SetBool("isAttacking", true);
        animator.SetInteger("attackIdx", skill.skillIdx);
        skill.usable = false;
    }

    public void movableSetter(bool flag) {
        movable = flag;
    }

    void Attack(Skills skill) {
        if (!animator.GetBool("isJumping")) {
            if (skill.usable) {
                AttackAnimation(skill);
                if(nearestEnemyInfo != null) {
                    nearestEnemyInfo.GetComponent<HitboxChecker>().getDamaged(skill.skillDamage);
                }
                if(skill.effect != null) {
                    Invoke(skill.effect, 0f);
                }

                audiosource.clip = attackSounds[UnityEngine.Random.Range(0, attackSounds.Length)];
                audiosource.Play();
                StartCoroutine(coolTimer(skill));
                
                IEnumerator coolTimer(Skills skill) {
                    yield return new WaitForSeconds(skill.skillCoolTime);
                    skill.usable = true;
                }

                StartCoroutine(MovableChecker());
                IEnumerator MovableChecker() {
                    movable = false;
                    yield return new WaitForSeconds(0.8f);
                    movable = true;
                }
            }
        }
    }

    public void SpawnHammer() {
        GameObject hammer = (GameObject)Instantiate(ThrowingWeapons[0], transform.position, Quaternion.identity);
        hammer.GetComponent<ObjectHammer>().SetDirection(spriteRenderer.flipX, gameObject);
    }

    // 피격 구현
    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "FireObject") {
            GetHit();
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }

        if(collision.tag == "Swip") {
            GetHit();
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }
        if(collision.tag == "Hit") {
            GetHit();
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }
        if(collision.gameObject.tag == "Pigeon") {
            GetHit();
            Destroy(collision.gameObject);
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }
        if(collision.tag == "Finish") {
            GameOver();
        }
    
        if(collision.gameObject.tag == "Item") {
            score += 1;
            gemScore.text = score.ToString() + " / " + gemsInGame.ToString();

            collision.gameObject.GetComponent<AudioSource>().Play();
            collision.gameObject.tag = "earnedItem";
            StartCoroutine(DeActive());
            IEnumerator DeActive() {
                for(int i = 0; i < 255; i++) {
                    collision.gameObject.GetComponent<SpriteRenderer>().color -= new Color(0f, 0f, 0f, 0.01f);
                    yield return new WaitForSeconds(0.01f);
                    
                }
                collision.gameObject.SetActive(false);
            }
        }

        if(collision.gameObject.tag == "Hitbox") {
            collision.gameObject.GetComponent<HitboxChecker>().cangetDamaged(true);
            nearestEnemyInfo = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Hitbox") {
            collision.gameObject.GetComponent<HitboxChecker>().cangetDamaged(false);
            nearestEnemyInfo = null;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            spriteRenderer.color = Color.red;
            int direction = (int)transform.position.x - 1;
            rigidBody.AddForce(new Vector2(direction, 0.3f) * 7);
            Invoke("OffDamaged", 2); // 3초 뒤
            float damage = collision.gameObject.GetComponent<SingleEnemy>().GetDamage();
            if (health > 0) {
                health -= damage;
                healthBar.fillAmount = health / 100f;
            }
        }

        if (collision.gameObject.tag == "Trab") {
            spriteRenderer.color = Color.red;
            int direction = (int)transform.position.x - 1;
            rigidBody.AddForce(new Vector2(direction, 0.3f) * 7);
            Invoke("OffDamaged", 2); // 3초 뒤
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }
        
        if (collision.gameObject.tag == "Boss") {
            GetHit();
            float damage = 10.0f;
            if (health > 0) {
                health -= damage;
                healthBar.fillAmount = health / 100f;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.gameObject.tag == "Slap") {
            GetHit();
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.tag == "showTime") {
            spriteRenderer.color = Color.red;
            if (health > 0) {
                health -= 1;
                healthBar.fillAmount = health / 100f;
            }
            Invoke("OffDamaged", 1);
        }

        if (collision.tag == "Horror") {
            if ((spriteRenderer.flipX == false && transform.position.x < 0) || (spriteRenderer.flipX == true && transform.position.x > 0)) {
                spriteRenderer.color = Color.red;
                if (health > 0) {
                    health -= 10;
                    healthBar.fillAmount = health / 100f;
                    movable = false;
                }

                StartCoroutine(OffHorror());
                IEnumerator OffHorror() {
                    yield return new WaitForSeconds(2.0f);
                    movable = true;
                    spriteRenderer.color = Color.white;
                }
            }
        }

        if (collision.tag == "Hook") {
            movable = false;
            transform.position = collision.bounds.center;
            StartCoroutine(OffHook());
            IEnumerator OffHook() {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, (float)rigidBody.velocity.normalized.y * 0f);
                yield return new WaitForSeconds(3.0f);
                gameObject.layer = 6;
                yield return new WaitForSeconds(1f);
                gameObject.layer = 0;
                movable = true;
            }
        }
    }

    void GetHit() {
        spriteRenderer.color = Color.red;
        int direction = (int)transform.position.x - 1;
        rigidBody.AddForce(new Vector2(direction, 0.3f) * 7, ForceMode2D.Impulse);
        Invoke("OffDamaged", 2); // 3초 뒤
    }

    void OffDamaged() {
        spriteRenderer.color = Color.white;
    }

    void GameOver() {
        StartCoroutine(SetOpacity());
        movable = false;

        IEnumerator SetOpacity() {
            while (gameOverPanel.GetComponent<Image>().color.a < 170.0f / 255.0f) {
                gameOverPanel.GetComponent<Image>().color += new Color(0.0f, 0.0f, 0.0f, 1.0f / 255.0f);
                yield return new WaitForSeconds(0.01f);
            }

            while (gameOverText.GetComponent<TextMeshProUGUI>().color.a < 1.0f) {
                gameOverText.GetComponent<TextMeshProUGUI>().color += new Color(0.0f, 0.0f, 0.0f, 1.2f / 255.0f);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(2.0f);
            ActiveRespawnBtn();
        }
    }

    void ActiveRespawnBtn(bool act = true) {
        respawnBtn.SetActive(act);
    }

    public void GotoStart(float posY) {
        this.gameObject.transform.position = new Vector3(0f, posY, 0f);
        gameOverPanel.GetComponent<Image>().color = new Color(255.0f, 161.0f, 0.0f, 0.0f);
        gameOverText.GetComponent<TextMeshProUGUI>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        ActiveRespawnBtn(false);
        movable = true;
    }
}