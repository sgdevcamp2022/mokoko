using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class LobbyCharactor : MonoBehaviour
{
    public TMP_Text nickNameText;

    PhotonView photon;
    
    void Awake() {
        photon = GetComponent<PhotonView>();
        nickNameText.text = photon.IsMine ? PhotonNetwork.NickName : photon.Owner.NickName;
    }

    public void SetNickName(string nickname) {
        // nickNameText.text = nickname;
    }
}