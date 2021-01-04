using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : Item {
    [SerializeField] private float _shitmassAmount = 10f;
    [SerializeField] private float _digestTime = 10f;

    public float ShitmassAmount {
        get {
            return _shitmassAmount;
        }
    }

    public float DigestTime {
        get {
            return _digestTime;
        }
    }
}
