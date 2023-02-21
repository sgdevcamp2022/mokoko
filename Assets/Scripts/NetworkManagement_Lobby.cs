using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManagement_Lobby : MonoBehaviourPunCallbacks
{
    public GameObject sceneController;
    
    GameObject blackBoard;
    string playerId;
    public int playerIdx;
    int playerNumbers;

    bool logIn = true;

    Vector3[] spritePosition = { new Vector3(-4.0f, -0.5f, 0.0f),
                                 new Vector3(-1.3f, -0.5f, 0.0f),
                                 new Vector3( 1.4f, -0.5f, 0.0f),
                                 new Vector3( 4.0f, -0.5f, 0.0f) };

    void Awake() {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        blackBoard = GameObject.Find("BlackBoard");
        playerId = blackBoard.GetComponent<BlackBoard>().ReturnPlayerId();
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) {
            PhotonNetwork.Disconnect();
        }
        
        if (Input.GetKeyDown(KeyCode.E) && PhotonNetwork.IsConnected) {
            Spawn();
        }

        if(logIn) {
            Connect();
            logIn = false;
        }
    }

    public override void OnJoinedRoom() {
        playerNumbers = PhotonNetwork.PlayerList.Length -1;
        PhotonNetwork.LocalPlayer.NickName = playerId;
        Spawn();
    }

    public void Spawn() {
        GameObject player = (GameObject)PhotonNetwork.Instantiate("LobbyCharactor_" + (playerNumbers+1).ToString(), spritePosition[playerNumbers], Quaternion.identity);
        blackBoard.GetComponent<BlackBoard>().playerIdx = playerNumbers;
        blackBoard.GetComponent<BlackBoard>().playerNumbers = playerNumbers;
        sceneController.GetComponent<LobbySceneController>().playerIdx = playerNumbers;
        sceneController.GetComponent<LobbySceneController>().player = player;
        sceneController.GetComponent<LobbySceneController>().Setter();
    }
}