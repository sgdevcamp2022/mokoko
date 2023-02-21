using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class PlatformPatterns : MonoBehaviourPunCallbacks
{
    public GameObject[] platform;
    public GameObject hook;
    public bool platformStop;
    
    Animator[] platformAnimationController;
    PhotonView photon;

    void Awake() {
        photon = GetComponent<PhotonView>();
        platformAnimationController = new Animator[platform.Length];
        for (int i = 0; i < platform.Length; i++) {
            platformAnimationController[i] = platform[i].GetComponent<Animator>();
            platform[i].SetActive(false);
        }
    }

    void Start() {
        platformcoroutine = Actor(2.5f, 3);
        
        if(PhotonNetwork.IsMasterClient) {
            StartCoroutine(platformcoroutine);
        }
    }

    void Update() {
        if (platformStop) {
            Stop(platformcoroutine);
        }
    }

    public void Stop(IEnumerator stopper) {
        StopCoroutine(stopper);
    }

    IEnumerator HookActor(float delay, int spawnNumber) {
        for (int i = 0; i < spawnNumber; i++) {
            float randomY = Random.Range(-0.5f, 0.5f);
            int direction = Random.Range(-1.0f, 1.0f) > 0 ? 1 : -1;

            GameObject spawnedHook = (GameObject)PhotonNetwork.Instantiate("Hook", new Vector3(direction * 15.0f, 2.5f + randomY, 0f), Quaternion.identity);

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator platformcoroutine;
    IEnumerator Actor(float Time, int activePlatform) {
        yield return new WaitForSeconds(2.5f);
        while (true) {
            int[] Platforms = GetActivePlatforms(activePlatform);
            for(int i = 0; i < Platforms.Length; i++) {
                photon.RPC("Active", RpcTarget.All, Platforms[i], true);
            }

            yield return new WaitForSeconds(Time);

            for (int i = 0; i < Platforms.Length; i++) {
                photon.RPC("Active", RpcTarget.All, Platforms[i], false);
            }
            yield return new WaitForSeconds(10.0f);
            StartCoroutine(HookActor(0.6f,5));
            yield return new WaitForSeconds(10.0f);
        }
    }

    [PunRPC]
    void Active(int activePlatform, bool trigger = true) {
        platform[activePlatform].SetActive(trigger);
        platformAnimationController[activePlatform].SetBool("ActiveFire", trigger);
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
