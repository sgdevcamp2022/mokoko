using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleSceneChanger : MonoBehaviour
{
    public GameObject player;
    public GameObject boss;
    public GameObject curtain;
    public GameObject storyTeller;
    public string nextStage;

    public string[] lines;
    public AudioClip[] endingAudio;

    public void SceneEnd() {
        if(curtain != null)
            curtain.GetComponent<Animator>().SetTrigger("SceneEnd");
        if(boss != null)
            boss.GetComponent<Animator>().SetTrigger("Dead");
        
        StartCoroutine(endingStoryAct());
        IEnumerator endingStoryAct() {
            int idx = 0;
            while(idx < lines.Length) {
                if(!storyTeller.GetComponent<AudioSource>().isPlaying) {
                    storyTeller.GetComponent<SingleStoryTeller>().StoryTellerActor(storyTeller.GetComponent<SingleStoryTeller>().classMaker(lines[idx], endingAudio[idx]));
                    idx++;
                }
                yield return new WaitForSeconds(1.0f);
            }
            
            yield return new WaitForSeconds(8.0f);
            if(nextStage.Length != 0)
                SceneManager.LoadScene(nextStage);
        }
    }
}