using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManagement : MonoBehaviourPunCallbacks
{
    public bool spawnBoss;
    public int playerIdx;

    GameObject blackBoard;
    bool innerChecker = true;
    string playerId;
    int playerNumbers;

    void Awake() {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        blackBoard = GameObject.Find("BlackBoard");

        playerId = blackBoard.GetComponent<BlackBoard>().ReturnPlayerId();
        playerNumbers = blackBoard.GetComponent<BlackBoard>().playerNumbers;
        playerIdx = blackBoard.GetComponent<BlackBoard>().playerIdx;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) {
            PhotonNetwork.Disconnect();
        }

        if(innerChecker) {
            Connect();
            innerChecker = false;
        }
    }

    public override void OnJoinedRoom() {
        Spawn();

        if(PhotonNetwork.IsMasterClient) {
            PhotonNetwork.Instantiate("Boss", new Vector3(0f, 0f, 0f), Quaternion.identity);
            spawnBoss = true;
        }
    }

    public void Spawn() {
        PhotonNetwork.LocalPlayer.NickName = playerId;
        PhotonNetwork.Instantiate("Player_" + (playerIdx +1).ToString(), new Vector3(Random.Range(-5f, 5f), 4, 0), Quaternion.identity);
    }
}