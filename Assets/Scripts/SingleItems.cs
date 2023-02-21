using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SingleItems : MonoBehaviour
{
    public bool active = true;

    Vector3 pos;
    void Awake() {
        pos = transform.position;
    }
}
