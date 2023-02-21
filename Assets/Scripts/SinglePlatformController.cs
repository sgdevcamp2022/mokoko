using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SinglePlatformController : MonoBehaviour
{
    public GameObject[] platform;
    public GameObject hook;
    public bool platformStop;

    Animator[] platformAnimationController;

    void Awake() {

        platformAnimationController = new Animator[platform.Length];
        for (int i = 0; i < platform.Length; i++) {
            platformAnimationController[i] = platform[i].GetComponent<Animator>();
            platform[i].SetActive(false);
        }
    }

    void Start() {
        platformcoroutine = Actor(2.5f, 3);
        StartCoroutine(platformcoroutine);
    }

    void Update() {
        if (platformStop) {
            Stop(platformcoroutine);
        }

    }

    public void Stop(IEnumerator stopper) {
        StopCoroutine(stopper);
    }

    IEnumerator hookcoroutine;
    IEnumerator HookActor(float delay, int spawnNumber) {
        for (int i = 0; i < spawnNumber; i++) {
            float randomY = Random.Range(-0.5f, 0.5f);
            int direction = Random.Range(-1.0f, 1.0f) > 0 ? 1 : -1;

            GameObject spawnedHook = (GameObject)Instantiate(hook, new Vector3(direction * 15.0f, 2.5f + randomY, 0f), Quaternion.identity);

            yield return new WaitForSeconds(delay);
        }
    }


    IEnumerator platformcoroutine;
    IEnumerator Actor(float Time, int activePlatform) {
        yield return new WaitForSeconds(2.5f);
        while (true) {
            int[] Platforms = GetActivePlatforms(activePlatform);
            for(int i = 0; i < Platforms.Length; i++) {
                Active(Platforms[i]);
            }

            yield return new WaitForSeconds(Time);

            for (int i = 0; i < Platforms.Length; i++) {
                DeActive(Platforms[i]);
            }
            yield return new WaitForSeconds(10.0f);
            StartCoroutine(HookActor(0.6f,5));
            yield return new WaitForSeconds(20.0f);
        }
    }

    void Active(int activePlatform) {
        platform[activePlatform].SetActive(true);
        platformAnimationController[activePlatform].SetBool("ActiveFire", true);
    }

    void DeActive(int activePlatform) {
        platform[activePlatform].SetActive(false);
        platformAnimationController[activePlatform].SetBool("ActiveFire", false);
    }

    int[] GetActivePlatforms(int activeNumbers) {
        int range = platform.Length;
        int[] activePlatforms = new int[activeNumbers];

        for (int i = 0; i < activePlatforms.Length; i++) {
            int selected = Random.Range(0, range);
            if (!activePlatforms.Contains(selected)) {
                activePlatforms[i] = selected;
            }
            else {
                i--;
            }
        }
        return activePlatforms;
    }
}
