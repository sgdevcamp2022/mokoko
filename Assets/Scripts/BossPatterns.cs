using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class BossPatterns : MonoBehaviourPunCallbacks, IPunObservable {
    public GameObject target;
    public Image bossHealthBar;
    public TMP_Text bossHealthDisplay;
 
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

    GameObject grid;
    GameObject slapPlatform;
    GameObject boss;
    GameObject clearPanel;
    GameObject clearPanelBackground;
    Animator bossController;
    AudioSource bossAudiosource;
    GameObject[] players;
    Image[] playerPanels;
    PhotonView photon;
    BoxCollider2D collider;

    
    void Awake() {
        boss = gameObject;
        bossController = boss.GetComponent<Animator>();
        bossAudiosource = boss.GetComponent<AudioSource>();
        photon = boss.GetComponent<PhotonView>();
        collider = GetComponent<BoxCollider2D>();
        slapPlatform = GameObject.FindGameObjectWithTag("Platform");
        grid = GameObject.FindGameObjectWithTag("Grid");
        clearPanel = GameObject.Find("Canvas").GetComponent<Transform>().Find("ClearPanel").gameObject;
        clearPanelBackground =  GameObject.Find("Canvas").GetComponent<Transform>().Find("ClearPanelBackground").gameObject;

        maxHP = bossHealth;

        if(PhotonNetwork.IsMasterClient)
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
            photon.RPC("PlatformTagSetter", RpcTarget.All, "Slap");
            yield return new WaitForSeconds(0.5f);
            photon.RPC("PlatformTagSetter", RpcTarget.All, "Platform");
        }
        StartCoroutine(CameraShake(5.5f + 0.316f, 0.2f, 0.3f));
        StartCoroutine(SoundEffect(5.7f, sounds.Laugh_3));
        
        StartCoroutine(Return(7.0f, new string[] { "ActPattern", "Grab", "Laugh", "Slap" }));
    }

    [PunRPC]
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
            photon.RPC("GridTagSetter", RpcTarget.All, "Horror");
            yield return new WaitForSeconds(0.02f);
            photon.RPC("GridTagSetter", RpcTarget.All, "Untagged");
        }
        StartCoroutine(CameraShake(4.0f + 0.1f, 0.3f, 0.3f));
        photon.RPC("panelColorChange", RpcTarget.All);
        StartCoroutine(SoundEffect(4.0f, sounds.Horror_1));
        StartCoroutine(Active(7.0f, "Laugh"));
        StartCoroutine(SoundEffect(7.0f, sounds.Laugh_2));

        StartCoroutine(Return(10.0f, new string[] { "ActPattern", "Turn", "Horror", "Laugh" }));
    }
    
    [PunRPC]
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

    [PunRPC]
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
                GameObject spawnedPigeon = (GameObject)PhotonNetwork.Instantiate("Pigeon", new Vector3(randomX, randomY, 0f), Quaternion.identity);
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
            photon.RPC("initiator", RpcTarget.All);
        }
        StartCoroutine(SoundEffect(0.0f, sounds.Etc));
    }

    [PunRPC]
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
            photon.RPC("BossTagSetter", RpcTarget.All, "LeftHand", "Hit");
            yield return new WaitForSeconds(1.0f);
            photon.RPC("BossTagSetter", RpcTarget.All, "LeftHand", "Untagged");
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
            photon.RPC("BossTagSetter", RpcTarget.All, "RightHand", "Hit");
            yield return new WaitForSeconds(0.3f);
            photon.RPC("BossTagSetter", RpcTarget.All, "RightHand", "Untagged");
        }
        StartCoroutine(Return(1.2f, new string[] { "ActPattern", "Hit" }));
    }

    [PunRPC]
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
        photon.RPC("BoolActor", RpcTarget.All, trigger, state);
    }

    [PunRPC]
    void BoolActor(string trigger, bool state) {
        bossController.SetBool(trigger, state);
    }

    [PunRPC]
    void TriggerActor(string trigger) {
        bossController.SetTrigger(trigger);
    }

    IEnumerator Return(float delayTime, string[] args) {
        yield return new WaitForSeconds(delayTime);

        while (true) {
            if (bossController.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) {
                for (int i = 0; i < args.Length; i++) {
                    photon.RPC("BoolActor", RpcTarget.All, args[i], false);
                }

                photon.RPC("TriggerActor", RpcTarget.All, "GotoIdle");
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
        photon.RPC("TriggerActor", RpcTarget.All, "Dead");
        StartCoroutine(SoundEffect(0.0f, sounds.Dead));
        GameObject.Find("Bundle").GetComponent<Transform>().Find("PatternController").GetComponent<PlatformPatterns>().platformStop = true;

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

            yield return new WaitForSeconds(1.0f);
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
        }

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players) {
            player.GetComponent<PlayerMove>().Record();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            stream.SendNext(bossHealth);
        } else {
            bossHealth = (float)stream.ReceiveNext();
        }
    }
}