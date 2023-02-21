using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public GameObject player;
    public float cameraSpeed;
    public Vector2 offset;
    public float limitMinX, limitMaxX, limitMinY, limitMaxY;

    float shakeAmount;
    float shakeTime;
    
    float height, width;

    void Start() {
        height = Camera.main.orthographicSize;
        width = height * Camera.main.aspect;
    }

    public void VibrateForTime(float time, float ref_shakeAmount) {
        shakeTime = time;
        shakeAmount = ref_shakeAmount;
    }

    void Update() {
        if(player != null) {
            if(shakeTime > 0) {
                transform.position = Random.insideUnitSphere * shakeAmount + this.transform.position;
                shakeTime -= Time.deltaTime;
            } else {
                shakeTime = 0.0f;

                Vector3 desiredPosition = new Vector3(Mathf.Clamp(player.transform.position.x + offset.x, limitMinX + width, limitMaxX - width),
                                                      Mathf.Clamp(player.transform.position.y + offset.y, limitMinY + height, limitMaxY - height),
                                                      -10.0f);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * cameraSpeed);
            }
        }
    }
}