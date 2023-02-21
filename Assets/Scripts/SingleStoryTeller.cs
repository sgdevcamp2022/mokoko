using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SingleStoryTeller : MonoBehaviour
{
    public GameObject storyText;
    public GameObject storyTextBackground;
    public Transform playerTransform;

    public string[] storyTexts;
    public AudioClip[] storySounds;
    public float[] storyPos;

    Coroutine runningCoroutine = null;
    Stories[] storyLine;
    AudioSource audiosource;

    public class Stories {
        public string storyText;
        public AudioClip storySound;
        public float duration;
        public bool postable;

        public Stories(string storyText, AudioClip storySound) {
            this.storyText = storyText;
            this.storySound = storySound;
            this.duration = 5.0f;
            this.postable = true;
        }
    }

    public Stories classMaker(string storyText, AudioClip storySound) {
        return new Stories(storyText, storySound);
    }

    void Awake() {
        Screen.SetResolution(960, 540, false);
        audiosource = GetComponent<AudioSource>();

        storyLine = new Stories[storyTexts.Length];
        for(int i = 0; i < storyTexts.Length; i++) {
            storyLine[i] = new Stories(storyTexts[i], storySounds[i]);
        }

        storyText.SetActive(false);
        storyTextBackground.SetActive(false);
    }

    void Update() {
        for(int i = 0; i < storyPos.Length; i++) {
            if(playerTransform.position.x > storyPos[i]) {
                StoryTellerActor(storyLine[i]);
            }
        }
    }
    
    public void StoryTellerActor(Stories story) {
        if(story.postable) {
            StartCoroutine(StoryTeller(story));
            story.postable = false;
        }
    }

    IEnumerator StoryTeller(Stories story) {
        yield return new WaitForSeconds(2.0f);
        while(audiosource.isPlaying) {
            yield return new WaitForSeconds(1.0f);
        }

        if(runningCoroutine != null) {
            StopCoroutine(runningCoroutine);
        }
        storyText.SetActive(true);
        storyTextBackground.SetActive(true);
        storyText.GetComponent<TextMeshProUGUI>().text = story.storyText;
        audiosource.clip = story.storySound;
        audiosource.Play();

        runningCoroutine = StartCoroutine(SetDeActive(story.duration));
    }

    IEnumerator SetDeActive(float duration) {
        yield return new WaitForSeconds(duration);
        storyText.SetActive(false);
        storyTextBackground.SetActive(false);
    }
}