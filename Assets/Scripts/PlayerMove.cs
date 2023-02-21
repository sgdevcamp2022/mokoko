using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPunCallbacks, IPunObservable {
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public GameObject respawnBtn;

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
    float clearTime = 0;

    Rigidbody2D rigidBody;
    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audiosource;
    PhotonView photon;
    GameObject clearPanel;

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
        photon = GetComponent<PhotonView>();

        nicknameText.text = photon.IsMine ? PhotonNetwork.NickName : photon.Owner.NickName;
        nicknameText.color = photon.IsMine ? Color.white : Color.yellow;
        if(photon.IsMine) {
            GameObject camera = GameObject.Find("CMCamera");
            if(camera != null) {
                camera.GetComponent<CameraShake>().player = gameObject;
                camera.GetComponent<CameraShake>().cameraSpeed = 5.0f;
            }
        }
    }

    void Update() {
        clearTime += Time.deltaTime;
        if(photon.IsMine) {
            if(movable) {
                if (Input.GetButtonUp("Horizontal")) {
                    rigidBody.velocity = new Vector2((float)rigidBody.velocity.normalized.x * 0.1f, rigidBody.velocity.y);
                }

                float axis = Input.GetAxisRaw("Horizontal");
                if (Input.GetButton("Horizontal")) {
                    photon.RPC("FlipX", RpcTarget.All, axis);
                    photon.RPC("Walk", RpcTarget.All, true);
                } else {
                    photon.RPC("Walk", RpcTarget.All, false);
                }

                if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping")) {
                    photon.RPC("JumpRPC", RpcTarget.All);
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
        } else {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
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

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && animator.GetBool("isAttacking")) {
            animator.SetBool("isAttacking", false);
        }
    }

    [PunRPC]
    void FlipX(float axis) {
        spriteRenderer.flipX = axis == -1;
    }

    [PunRPC]
    void Walk(bool setter) {
        animator.SetBool("isWalking", setter);
    }

    [PunRPC]
    void JumpRPC() {
        rigidBody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        audiosource.clip = jumpSounds[Random.Range(0, jumpSounds.Length)];
        audiosource.Play();
    }

    [PunRPC]
    void AttackAnimation(bool skillUsable, int skillIdx) {
        animator.SetBool("isAttacking", true);
        animator.SetInteger("attackIdx", skillIdx);
    }

    [PunRPC]
    void bossGetHit(float damage) {
        BossPatterns boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossPatterns>();
        boss.bossHealth -= damage;
    }

    void Attack(Skills skill) {
        if (!animator.GetBool("isJumping")) {
            if (skill.usable) {
                BossPatterns boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossPatterns>();
                if (boss.canDamaged) {
                    photon.RPC("bossGetHit", RpcTarget.All, skill.skillDamage);
                }
                photon.RPC("AttackAnimation", RpcTarget.All, skill.usable, skill.skillIdx);
                PhotonNetwork.SendAllOutgoingCommands();
                skill.usable = false;
                if(skill.effect != null) {
                    Invoke(skill.effect, 0f);
                }

                audiosource.clip = attackSounds[Random.Range(0, attackSounds.Length)];
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
        if(collision.tag == "FireObject") {
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
        if(photon.IsMine) {
            if(collision.tag == "Finish") {
                GameOver();
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        if(collision.gameObject.tag == "Slap") {
            GetHit();
            if (health > 0) {
                health -= 10;
                healthBar.fillAmount = health / 100f;
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if(collision.tag == "showTime") {
            spriteRenderer.color = Color.red;
            if (health > 0) {
                health -= 1;
                healthBar.fillAmount = health / 100f;
            }
            Invoke("OffDamaged", 1);
        }

        if(collision.tag == "Horror") {
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
        
        bool catchable = true;
        if(collision.tag == "Hook") {
            if(catchable) {
                movable = false;
                catchable = false;
                transform.position = collision.bounds.center;
                StartCoroutine(OffHook());
                IEnumerator OffHook() {
                    rigidBody.velocity = new Vector2(rigidBody.velocity.x, (float)rigidBody.velocity.normalized.y * 0f);
                    yield return new WaitForSeconds(3.0f);
                    gameObject.layer = 6;
                    yield return new WaitForSeconds(1.0f);
                    gameObject.layer = 0;
                    catchable = true;
                    movable = true;
                }
            }
        }
    }
   
    void GetHit() {
        spriteRenderer.color = Color.red;
        int direction = (int)transform.position.x - 1;
        rigidBody.AddForce(new Vector2(direction, 0.3f) * 7, ForceMode2D.Impulse);
        Invoke("OffDamaged", 2);
    }
     
    void OffDamaged() {
        spriteRenderer.color = Color.white;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(health);
        } else {
            curPos = (Vector3)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();
        }
    }

    void GameOver() {
        movable = false;
        animator.SetTrigger("isDie");
        StartCoroutine(SetOpacity());

        IEnumerator SetOpacity() {
            while (gameOverPanel.GetComponent<Image>().color.a < 170.0f / 255.0f) {
                gameOverPanel.GetComponent<Image>().color += new Color(0.0f, 0.0f, 0.0f, 1.0f / 255.0f);
                yield return new WaitForSeconds(0.01f);
            }
            while (gameOverText.GetComponent<TextMeshProUGUI>().color.a < 1.0f) {
                gameOverText.GetComponent<TextMeshProUGUI>().color += new Color(0.0f, 0.0f, 0.0f, 1.2f / 255.0f);
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void Record() {
        GameObject blackboard = GameObject.Find("BlackBoard");
        blackboard.GetComponent<BlackBoard>().SettupRecords(health, clearTime);
        StartCoroutine(SceneChanger());
        IEnumerator SceneChanger() {
            yield return new WaitForSeconds(25.0f);
            PhotonNetwork.Disconnect();
            blackboard.GetComponent<BlackBoard>().SceneChanger("EndingScene");
        }
    }

    void ActiveRespawnBtn() {
        respawnBtn.SetActive(true);
    }

    public void GotoLobby() {
        SceneManager.LoadScene("LobbyScene");
    }
}
