using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;
using Unity.VisualScripting;

public class EndingSceneManager : MonoBehaviour
{
    public GameObject scoresContent;
    public GameObject creditContent;
    public GameObject BlackBoard;

    public GameObject endingContainer;

    private DatabaseReference databaseRef;

    string[,] credits = new string[,] {
        {"2023.02", "Team Mokoko in 2022 SmileGate WinterDevCamp"},
        {"Team Mokoko", "Kim YoungJun, Jang SeokWon, Kim KangLin"},
        {"CAMP LEADER", "Kim KangLin"},
        {"Lead Programmer", "Jang SeokWon"},
        {"FRONTEND PROGRAMMERS", "Jang SeokWon, Kim KangLin"},
        {"BACKEND - PHOTON PROGRAMMERS", "Kim KangLin"},
        {"BACKEND - FIREBASE PROGRAMMERS", "Kim YoungJun"},
        {"SOUNDs", " from SmileGate RPG - LOSTARK"},
        {"Story", "Kim KangLin"},
        {"Assets", "Jang SeokWon"},
        {"Level Design", "Kim KangLin"},
        {"Testers", "Park ChanWhee, Lee SangYeop"},
        {"Supported by ", "SmileGate WinterDevCamp"},
        {"", "Thankyou For Playing"}
    };

    
    public class Data
    {
        public string playerName;
        public float clearTime;

        public Data(string playerName, float clearTime)
        {
            this.playerName = playerName;
            this.clearTime = clearTime;
        }
    }
    
    public void SaveUser(){        
        for (int i = 0; i < 15; i++) {
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            string playerName = "Temp User " + i;
            float clearTime = Random.Range(0.0f, 10.0f);

            var data = new Data(playerName, clearTime);
            string jsonData = JsonUtility.ToJson(data);

            databaseRef.Child("clearinfo").Child(playerName).SetRawJsonValueAsync(jsonData).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Save user data was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Save user data encountered an error: " + task.Exception);
                    return;
                }
        
                Debug.LogFormat("Save user data in successfully");
            });
        }
    }



    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot

        Debug.Log(args.Snapshot.ChildrenCount);

        foreach (var child in args.Snapshot.Children)
        {
            /*Debug.Log(child.ToString());
            Debug.Log(child.Child("playerName").Value);
            Debug.Log(child.Child("clearTime").Value);
            Debug.Log("================");*/


            //string playerName = BlackBoard.GetComponent<BlackBoard>().ReturnPlayerId() + i;
            string playerName = child.Key;
            string clearTime = child.Child("clearTime").Value.ToString();

            GameObject Container = (GameObject)Instantiate(endingContainer, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            Container.transform.SetParent(scoresContent.transform, false);
            Container.GetComponent<EndingContainer>().Setter(playerName, clearTime);
            

        }
        
    }

    void Print (string s)
    {
        Debug.Log(s.ToString());
    }

    void Awake() {

        Screen.SetResolution(960, 540, false);

        Debug.Log("Ending Awake");

        if (scoresContent != null) {
            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            string playerName = BlackBoard.GetComponent<BlackBoard>().ReturnPlayerId();
            if (playerName.Equals(""))
            {
                playerName = "basicPlayerName";
            }
            
            /*Fix this line to real clearTime*/
            float clearTime = 0.0f;
            /*Fix this line to real clearTime*/

            var data = new Data(playerName, clearTime);
            string jsonData = JsonUtility.ToJson(data);

            

            databaseRef.Child("clearinfo").Child(playerName).SetRawJsonValueAsync(jsonData).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Save user data was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("Save user data encountered an error: " + task.Exception);
                    return;
                }

                Debug.LogFormat("Save user data in successfully");
            });
            
            

            //SaveUser();

            FirebaseDatabase.DefaultInstance.GetReference("clearinfo").OrderByChild("clearTime")
                .LimitToFirst(10).ValueChanged += HandleValueChanged;

            
        }

        if(creditContent != null) {
            for(int i = 0; i < credits.GetLength(0); i++) {
                GameObject Container = (GameObject)Instantiate(endingContainer, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                Container.transform.SetParent(creditContent.transform, false);
                Container.GetComponent<EndingContainer>().Setter(credits[i,0], credits[i,1], TextAlignmentOptions.Center);
            }
        }

    }

    void Update() {
        Transform credits = creditContent.GetComponent<Transform>();
        credits.position += new Vector3(0.0f, Time.deltaTime, 0.0f);
        
        
    }
}
