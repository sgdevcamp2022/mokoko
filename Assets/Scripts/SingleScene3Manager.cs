using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleScene3Manager : MonoBehaviour
{
    void Update() {
    
    }


    void OnTriggerStay2D(Collider2D collision) {
        SceneManager.LoadScene("SingleStage_4");
    }
}
