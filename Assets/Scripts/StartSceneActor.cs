using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Extensions;
using Firebase.Database;

public class StartSceneActor : MonoBehaviour
{
    public GameObject BlackBoard;
    public GameObject CreateSpace;
    public GameObject LoginSpace;
    public GameObject startBtn;
    public GameObject singleBtn;
    public GameObject multiBtn;

    public TMP_InputField EmailInputField1;
    public TMP_InputField PasswordInputField1;
    public TMP_Text OutputText1;
    public TMP_InputField EmailInputField2;
    public TMP_InputField UsernameInputField2;
    public TMP_InputField PasswordInputField2;

    public FirebaseApp firebaseApp;
    private DatabaseReference databaseRef;

    public void ActivateLogin() {
        if(!LoginSpace.activeSelf){
            LoginSpace.SetActive(true);
        } else {
            LoginSpace.SetActive(false);
        }
    }

    public void ActivateCreate() {
        LoginSpace.SetActive(false);
        CreateSpace.SetActive(true);
    }

    public void Back() {
        CreateSpace.SetActive(false);
    }

    void Awake() {
        if (FirebaseAuthController.Instance.UserId != "") {
            SignOut();
        }        

        FirebaseAuthController.Instance.InitializeFirebase();                
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        /*var data = new Data("check@naver.com", "checkUser");
        string jsonData = JsonUtility.ToJson(data);
        databaseRef.Child("name").SetRawJsonValueAsync(jsonData);*/
        Screen.SetResolution(960, 540, false);
    }

    public void SignIn() {        
        string email = EmailInputField1.text;
        string password = PasswordInputField1.text;
        string reformEmail = email.Substring(0, email.IndexOf('@'));
        string userName = "";
        
        EmailInputField1.text = "";
        PasswordInputField1.text = "";

        FirebaseDatabase.DefaultInstance.GetReference("users")
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.Log("Error exists while Read Users Data");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot= task.Result;                    
                    userName = snapshot.Child(reformEmail).Child("stats").Child("username").Value.ToString().Trim();                    
                    BlackBoard.GetComponent<BlackBoard>().updatePlayerId(userName);                    
                }
            });
        
        FirebaseAuthController.Instance.SignIn(email, password); 
        FirebaseAuthController.Instance.OnChangedLoginState += OnChangedLoginState;
    }

    public void SignOut() {
        FirebaseAuthController.Instance.OnChangedLoginState += OnChangedLoginState;
        FirebaseAuthController.Instance.SignOut();
        FirebaseAuthController.Instance.OnChangedLoginState += OnChangedLoginState;
        Debug.Log("SignOut1");
    }

    public class Data
    {
        public string email;
        public string username;

        public Data(string email, string username)
        {
            this.email = email;
            this.username = username;
        }
    }

    public void CreateUser() {
        string curEmail = EmailInputField2.text;
        string username = UsernameInputField2.text;
        string password = PasswordInputField2.text;
        string reformEmail = curEmail.Substring(0, curEmail.IndexOf('@'));
        
        EmailInputField2.text = "";
        UsernameInputField2.text = "";
        PasswordInputField2.text = "";
        FirebaseAuthController.Instance.CreateUser(curEmail, password);
        
        var data = new Data(curEmail, username);
        string jsonData = JsonUtility.ToJson(data);

        databaseRef.Child("users").Child(reformEmail).Child("stats").SetRawJsonValueAsync(jsonData).ContinueWith(task =>
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

        CreateSpace.SetActive(false);
        LoginSpace.SetActive(true);        
    }

    private void OnChangedLoginState(bool sighneIn) {
        OutputText1.text = sighneIn ? "Loading..." : "Welcome";
        // OutputText.text += FirebaseAuthController.Instance.UserId;
        if (sighneIn){
            Debug.Log("SceneChagned to LobbyScene");
            SceneChanger("LobbyScene");
        }
        else {
            SceneChanger("StartScene");
        }
    }

    public void SceneChanger(string whereTo) {
        SceneManager.LoadScene(whereTo);
    }
}