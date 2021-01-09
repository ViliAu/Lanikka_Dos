using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {
    
    [SerializeField] private Vector3 startPoint = Vector3.zero;
    [SerializeField] private Vector3 endPoint = Vector3.zero;
    [SerializeField] private int segments = 10;
    [SerializeField] private float looseness = 1f;

    private LineRenderer lr = null;

    private void Awake() {
        lr = GetComponent<LineRenderer>();
        if (lr == null) {
            Debug.LogError("No linerenderer found");
            return;
        }
        SetupRenderer();
    }

    private void SetupRenderer() {
        lr.positionCount = segments;
        Vector3 unitVector = (endPoint - startPoint) / (segments-1);
        for (int i = 0; i < segments; i++) {
            Vector3 looseNessVector = Vector3.down * Mathf.Sin(((float)i/segments) * Mathf.PI) * looseness;
            lr.SetPosition(i, startPoint + unitVector * i + looseNessVector);
        }
    }


}
