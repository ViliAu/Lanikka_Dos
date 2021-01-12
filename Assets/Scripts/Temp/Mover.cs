using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {

    float a = 0;
    public Vector3 moveVec = Vector3.zero;

    void Update() {
        a += 0.005f;
        moveVec = Vector3.up * Mathf.Sin(a) * 5; 
        transform.Translate(moveVec * Time.deltaTime);
    }

}
