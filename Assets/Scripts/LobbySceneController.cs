using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbySceneController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Button[] containers;
    public GameObject[] playerPrefet;
    public Sprite[] readySprites;
    public GameObject countdownText;

    public int playerIdx = 0;
    
    public bool[] readyChecker = { false, false, false, false };
    public bool[] inPlayer = { false, false, false, false };
    public bool[] avaliable = { true, true, true, true };
    Vector3[] spritePosition = { new Vector3(-4.0f, -0.5f, 0.0f),
                                 new Vector3(-1.3f, -0.5f, 0.0f),
                                 new Vector3( 1.4f, -0.5f, 0.0f),
                                 new Vector3( 4.0f, -0.5f, 0.0f) };
    Image[] containerImages;

    PhotonView photon;
    public GameObject player;

    void Awake() {
        photon = GetComponent<PhotonView>();

        containerImages = new Image[containers.Length];
        for(int i = 0; i < containerImages.Length; i++) {
            containerImages[i] = containers[i].GetComponent<Image>();
        }
    }

    void Update() {
        GetAvaliable();
    }

    [PunRPC]
    void InPlayerSetter(int idx, bool state) {
        inPlayer[idx] = state;
    }

    [PunRPC]
    void ReadyCheckerSetter(int idx, bool state) {
        readyChecker[idx] = state;
    }

    [PunRPC]
    void AvaliableSetter(int idx, bool state) {
        avaliable[idx] = state;
    }

    public void StateChecker() {
        if (readyChecker[playerIdx]) {
            photon.RPC("ReadyCheckerSetter", RpcTarget.All, playerIdx, false);
            readyChecker[playerIdx] = false;
            SceneChanger();
        } else {
            photon.RPC("ReadyCheckerSetter", RpcTarget.All, playerIdx, true);
            readyChecker[playerIdx] = true;
            SceneChanger();
        }
    }

    public void Avaliable(int idx) {
        if(!inPlayer[idx]) {
            if(PhotonNetwork.IsMasterClient) {
                if(avaliable[idx]) {
                    photon.RPC("AvaliableSetter", RpcTarget.All, idx, false);
                    photon.RPC("ReadyCheckerSetter", RpcTarget.All, idx, true);
                    SceneChanger();
                } else {
                    photon.RPC("AvaliableSetter", RpcTarget.All, idx, true);
                    photon.RPC("ReadyCheckerSetter", RpcTarget.All, idx, false);
                    SceneChanger();
                }
            }
        }
    }

    public void Setter() {
        photon.RPC("ReadyCheckerSetter", RpcTarget.All, playerIdx, false);
        photon.RPC("InPlayerSetter", RpcTarget.All, playerIdx, true);
        photon.RPC("AvaliableSetter", RpcTarget.All, playerIdx, false);
    }

    [PunRPC]
    void SpawnCharactor() {
        readyChecker[playerIdx] = false;
        inPlayer[playerIdx] = true;
        avaliable[playerIdx] = false;
    }

    void GetAvaliable() {
        for (int i = 0; i < readyChecker.Length; i++) {
            if(!avaliable[i] && !readyChecker[i] || avaliable[i] && !readyChecker[i]) {
                containerImages[i].sprite = readySprites[0];
            } else if(!avaliable[i] && readyChecker[i] && !inPlayer[i]) {
                containerImages[i].sprite = readySprites[2];
            } else {
                containerImages[i].sprite = readySprites[1];
            }
        }
    }

    [PunRPC]
    void CountDown() {
        StartCoroutine(countDown());
        IEnumerator countDown() {
            countdownText.SetActive(true);
            TextMeshProUGUI txt = countdownText.GetComponent<TextMeshProUGUI>();

            yield return new WaitForSeconds(3.0f);
    
            GetComponent<AudioSource>().Play();
            txt.text = "3";
            yield return new WaitForSeconds(1.0f);
            
            txt.text = "2";
            yield return new WaitForSeconds(1.0f);
            
            txt.text = "1";
            yield return new WaitForSeconds(1.0f);
            
            Destroy(player);
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("MainScene");
        }
    }

    void SceneChanger() {
        int checker = 0;
        foreach(bool state in readyChecker) {
            if(state) {
                checker += 1;
            }
        }
        if(checker > 3) {
            photon.RPC("CountDown", RpcTarget.All);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting) {
            for(int i = 0; i < readyChecker.Length; i++) {
               stream.SendNext(this.readyChecker[i]); 
               stream.SendNext(this.inPlayer[i]);
               stream.SendNext(this.avaliable[i]);
            }
        } else {
            for(int i = 0; i < readyChecker.Length; i++) {
               readyChecker[i] = (bool)stream.ReceiveNext();
               inPlayer[i] = (bool)stream.ReceiveNext();
               avaliable[i] = (bool)stream.ReceiveNext();
            }
        }
    }
}