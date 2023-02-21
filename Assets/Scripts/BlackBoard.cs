using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BlackBoard : MonoBehaviour
{
    public string playerId;
    public int playerIdx = 0;
    public int playerNumbers;
    string playerName;
    float remainHP;
    float clearTime;
    
    void Awake() {
        playerId = "basicPlayerName";
    }

    public void updatePlayerId(string id) {
        playerId = id;
    }

    public string ReturnPlayerId() {
        return playerId;
    }
    
    public void SettupRecords(float remainhp, float damage) {
        remainHP = remainhp;
        clearTime = damage;
    }

    public float GetRemainHp() {
        return remainHP;
    }
    
    public float GetClearTime() {
        return clearTime;
    }

    public void SignOut() {   
        FirebaseAuthController.Instance.SignOut();
        FirebaseAuthController.Instance.OnChangedLoginState += OnChangedLoginState;    
    }

    private void OnChangedLoginState(bool sighneIn) {
        if (!sighneIn){
            SceneChanger("StartScene");
        }
    }
    
    public void SceneChanger(string whereTo) {
        SceneManager.LoadScene(whereTo);
    }
}