using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour {
    
    [SerializeField] private Vector3 startPoint = Vector3.zero;
    [SerializeField] private Vector3 endPoint = Vector3.zero;
    [SerializeField] private int segments = 10;

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
        lr.SetPosition(0, startPoint);
        Vector3 unitVector = (endPoint - startPoint) / (segments-1);
        for (int i = 1; i < segments; i++) {
            lr.SetPosition(i, startPoint + unitVector * i);
        }
    }


}
