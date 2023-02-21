using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SingleBossPattern : MonoBehaviour {
    public GameObject pigeon;
    public GameObject target;
    public GameObject grid;
    public GameObject slapPlatform;
    public Image bossHealthBar;
    public TMP_Text bossHealthDisplay;
    public GameObject clearPanel;
    public GameObject clearPanelBackground;
    public GameObject hitbox;
 
    public AudioClip[] bossSounds;
    public AudioClip[] clearSound;
    enum sounds {
        Horror_1,
        Laugh_1,
        Laugh_2,
        Laugh_3,
        Roar_1,
        Roar_2,
        Pigeon,
        Etc,
        Dead
    }
    
    public bool canDamaged = false;
    public bool bossDead = false;
    public float bossHealth;

    enum patterns {
        Slap,
        Horror,
        Pigeon,
        Showtime,
        Swip,
        Hit
    }

    bool patterning = false;
    float maxHP;

    GameObject boss;
    Animator bossController;
    AudioSource bossAudiosource;
    GameObject[] players;
    Image[] playerPanels;
    BoxCollider2D collider;

    void Awake() {
        boss = gameObject;
        bossController = boss.GetComponent<Animator>();
        bossAudiosource = boss.GetComponent<AudioSource>();
        collider = GetComponent<BoxCollider2D>();

        bossHealth = hitbox.GetComponent<HitboxChecker>().maxHP;
        maxHP = bossHealth;

        StartCoroutine(BossCoroutine());
    }

    void PlayerInfoSetter() {
        playerPanels = new Image[players.Length];

        for (int i = 0; i < players.Length; i++) {

            GameObject Canvas = players[i].GetComponent<Transform>().Find("Canvas").gameObject;
            GameObject playerPanel = Canvas.GetComponent<Transform>().Find("PlayerPanel").gameObject;
            playerPanels[i] = playerPanel.GetComponent<Image>();
        }
    }

    void Update() {
        players = GameObject.FindGameObjectsWithTag("Player");
        PlayerInfoSetter();

        bossHealth = hitbox.GetComponent<HitboxChecker>().GetHP();
        bossHealthBar.fillAmount = bossHealth / maxHP;
        bossHealthDisplay.text = bossHealth.ToString();

        if(bossHealth < 0) {
            if(!bossDead) {
                bossHealth = 0;
                bossDead = true;
                BossDead();
            }
        }
    }

    public void PatternSlap() {
        patterning = true;
        StartCoroutine(Active(0.0f, "ActPattern"));
        StartCoroutine(Active(0.0f, "Grab"));
        StartCoroutine(Active(3.0f, "Laugh"));
        StartCoroutine(SoundEffect(3.0f, sounds.Laugh_1));
        StartCoroutine(Active(5.5f, "Slap"));
        StartCoroutine(SlapDamage(5.7f));
        IEnumerator SlapDamage(float delayTime) {
            yield return new WaitForSeconds(delayTime);
            PlatformTagSetter("Slap");
            yield return new WaitForSeconds(0.5f);
            PlatformTagSetter("Platform");
        }
        StartCoroutine(CameraShake(5.5f + 0.316f, 0.2f, 0.3f));
        StartCoroutine(SoundEffect(5.7f, sounds.Laugh_3));
        
        StartCoroutine(Return(7.0f, new string[] { "ActPattern", "Grab", "Laugh", "Slap" }));
    }

    void PlatformTagSetter(string tagName) {
        slapPlatform.tag = tagName;
    }

    public void PatternHorror() {
        patterning = true;
        StartCoroutine(Active(0.0f, "ActPattern"));
        StartCoroutine(Active(0.0f, "Turn"));
        StartCoroutine(Active(4.0f, "Horror"));
        StartCoroutine(HorrorDamage(4.0f));
        IEnumerator HorrorDamage(float delayTime) {
            yield return new WaitForSeconds(delayTime);
            GridTagSetter("Horror");
            yield return new WaitForSeconds(0.02f);
            GridTagSetter("Untagged");
        }
        StartCoroutine(CameraShake(4.0f + 0.1f, 0.3f, 0.3f));
        panelColorChange();
        StartCoroutine(SoundEffect(4.0f, sounds.Horror_1));
        StartCoroutine(Active(7.0f, "Laugh"));
        StartCoroutine(SoundEffect(7.0f, sounds.Laugh_2));

        StartCoroutine(Return(10.0f, new string[] { "ActPattern", "Turn", "Horror", "Laugh" }));
    }
    
    void panelColorChange() {
        StartCoroutine(ColorChanger(4.0f + 0.1f, 2.0f, 3.0f, new Color (1.0f, 4/255f, 171/255f, 51/255f)));
        IEnumerator ColorChanger(float delayTime, float duration, float revokeTime, Color color) {
            yield return new WaitForSeconds(delayTime);
            
            foreach(Image panel in playerPanels) {
                panel.color = color;

                yield return new WaitForSeconds(duration);

                while (panel.color[3] > 0) {
                    panel.color -= new Color(0, 0, 0, color[3] / revokeTime);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    void GridTagSetter(string tagName) {
        grid.tag = tagName;
    }

    public void PatternPigeon() {
        patterning = true;
        StartCoroutine(Active(0.0f, "ActPattern"));
        StartCoroutine(Active(0.0f, "Move"));
        StartCoroutine(Pigeons(2.0f));
        IEnumerator Pigeons(float delayTime) {
            yield return new WaitForSeconds(delayTime);

            for (int i = 0; i < 10; i++) {
                SpawnPigeon();
                yield return new WaitForSeconds(0.1f);
            }

            GameObject SpawnPigeon() {
                float basicX = -9.0f;

                float randomX = UnityEngine.Random.Range(basicX - 0.5f, basicX + 0.5f);
                float randomY = UnityEngine.Random.Range(-2.0f, 1.0f);
                GameObject spawnedPigeon = (GameObject)Instantiate(pigeon, new Vector3(randomX, randomY, 0f), Quaternion.identity);
                return spawnedPigeon;
            }
        }
        StartCoroutine(SoundEffect(2.0f, sounds.Pigeon));
        StartCoroutine(Active(4.0f, "Back"));
        StartCoroutine(Active(5.5f, "Laugh"));
        StartCoroutine(SoundEffect(5.5f, sounds.Laugh_1));

        StartCoroutine(Return(7.0f, new string[] { "ActPattern", "Move", "Back", "Laugh" }));
    }

    public void PatternShowtime() {
        float randomX = UnityEngine.Random.Range(this.transform.position.x - 1.0f, this.transform.position.x + 1.0f);
        float randomY = UnityEngine.Random.Range(this.transform.position.y - 1.0f, this.transform.position.y + 1.0f);

        foreach(GameObject player in players) {
            initiator();
        }
        StartCoroutine(SoundEffect(0.0f, sounds.Etc));
    }

    void initiator() {
        foreach(GameObject player in players) {
            spawnTarget().GetComponent<ObjectTarget>().init(player.GetComponent<Transform>());
        }
        
        GameObject spawnTarget() {
            GameObject spawnedTarget = (GameObject)Instantiate(target, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            return spawnedTarget;
        }
    }

    public void PatternSwip() {
        patterning = true;
        StartCoroutine(Active(0.0f, "ActPattern"));
        StartCoroutine(Active(0.0f, "Swip"));
        StartCoroutine(SoundEffect(0.1f, sounds.Roar_2));
        StartCoroutine(SwipDamage(0.7f));
        IEnumerator SwipDamage(float delayTime) {
            yield return new WaitForSeconds(delayTime);
            BossTagSetter("LeftHand", "Hit");
            yield return new WaitForSeconds(1.0f);
            BossTagSetter("LeftHand", "Untagged");
        }
        StartCoroutine(Return(1.0f, new string[] { "ActPattern", "Swip" }));
    }

    public void PatternHit() {
        patterning = true;
        StartCoroutine(Active(0.0f, "ActPattern"));
        StartCoroutine(Active(0.0f, "Hit"));
        StartCoroutine(SoundEffect(0.1f, sounds.Roar_2));
        StartCoroutine(HitDamage(0.7f));
        IEnumerator HitDamage(float delayTime) {
            yield return new WaitForSeconds(delayTime);
            BossTagSetter("RightHand", "Hit");
            yield return new WaitForSeconds(0.3f);
            BossTagSetter("RightHand", "Untagged");
        }
        StartCoroutine(Return(1.2f, new string[] { "ActPattern", "Hit" }));
    }

    void BossTagSetter(string hand, string tagName) {
        boss.transform.Find(hand).gameObject.tag = tagName;
    }

    void OnTriggerStay2D(Collider2D collision) {
        if(collision.tag == "Player") {
            canDamaged = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            canDamaged = false;
        }
    }

    IEnumerator BossCoroutine() {
        int nextPattern = UnityEngine.Random.Range(1,7);
        int currentPattern = -1;
        yield return new WaitForSeconds(5.0f);
        while (!bossDead) {
            if(!patterning) {
                if (nextPattern == 1) {
                    PatternSlap();
                    PatternEnd();
                    yield return new WaitForSeconds(10.0f);
                
                } else if (nextPattern == 2) {
                    PatternHorror();
                    PatternEnd();
                    yield return new WaitForSeconds(15.0f);

                } else if (nextPattern == 3) {
                    collider.enabled = false;
                    PatternPigeon();
                    yield return new WaitForSeconds(5.0f);
                    collider.enabled = true;
                    PatternEnd();
                    yield return new WaitForSeconds(10.0f);
                    
                } else if (nextPattern == 4) {
                    PatternShowtime();
                    PatternEnd();
                    yield return new WaitForSeconds(7.0f);

                } else if (nextPattern == 5) {
                    PatternSwip();
                    PatternEnd();
                    yield return new WaitForSeconds(7.0f);

                } else if (nextPattern == 6) {
                    PatternHit();
                    PatternEnd();
                    yield return new WaitForSeconds(7.0f);

                }
            }
            yield return new WaitForSeconds(0.05f);
        }
        void PatternEnd() {
            StartCoroutine(PickNextPattern());
            IEnumerator PickNextPattern() {
                do {
                    nextPattern =  UnityEngine.Random.Range(1,7);
                    yield return new WaitForSeconds(0.01f);
                } while(nextPattern == currentPattern);
                currentPattern = nextPattern;
            }
        }
    }

    IEnumerator CameraShake(float delayTime, float shakeTime, float shakeAmount) {
        yield return new WaitForSeconds(delayTime);
    }

    IEnumerator Active(float delayTime, string trigger, bool state = true) {
        yield return new WaitForSeconds(delayTime);
        BoolActor(trigger, state);
    }

    void BoolActor(string trigger, bool state) {
        bossController.SetBool(trigger, state);
    }

    void TriggerActor(string trigger) {
        bossController.SetTrigger(trigger);
    }

    IEnumerator Return(float delayTime, string[] args) {
        yield return new WaitForSeconds(delayTime);

        while (true) {
            if (bossController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
                for (int i = 0; i < args.Length; i++) {
                    BoolActor(args[i], false);
                }

                TriggerActor("GotoIdle");
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
        patterning = false;
    }

    IEnumerator SoundEffect(float delayTime, sounds sound) {
        yield return new WaitForSeconds(delayTime);
        bossAudiosource.clip = bossSounds[(int)sound];
        bossAudiosource.Play();
    }

    void BossDead() {
        TriggerActor("Dead");
        StartCoroutine(SoundEffect(0.0f, sounds.Dead));

        clearPanel.SetActive(true);
        Transform panelTransform = clearPanel.GetComponent<Transform>();
        Image panelImage = clearPanel.GetComponent<Image>();
        AudioSource panelAudio = clearPanel.GetComponent<AudioSource>();

        clearPanelBackground.SetActive(true);
        Image clearPanelBackgroundImage = clearPanelBackground.GetComponent<Image>();

        StartCoroutine(PanelActive(panelTransform, panelImage, panelAudio, clearPanelBackgroundImage));
        IEnumerator PanelActive(Transform transform, Image image, AudioSource audio, Image backgroundImage) {
            yield return new WaitForSeconds(10.0f);

            audio.clip = clearSound[0];
            audio.Play();

            yield return new WaitForSeconds(0.5f);
            
            while(backgroundImage.color[3] < 0.4f) {
                backgroundImage.color += new Color(0.0f, 0.0f, 0.0f, 0.15f);
                yield return new WaitForSeconds(0.01f);
            }

            while(transform.localScale.x > 4.0f) {
                transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
                image.color += new Color(0.0f, 0.0f, 0.0f, 0.17f);
                yield return new WaitForSeconds(0.01f);
            }
            
            while(audio.isPlaying) {
                yield return new WaitForSeconds(1.0f);
            }
            audio.clip = clearSound[1];
            audio.Play();

            while(audio.isPlaying) {
                yield return new WaitForSeconds(1.0f);
            }
            yield return new WaitForSeconds(3.0f);
            SceneManager.LoadScene("SingleStage_5_Ending");
            
        }
    }
}