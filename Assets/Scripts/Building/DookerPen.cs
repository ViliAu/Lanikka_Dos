using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DookerPen : MonoBehaviour {
    [Header("Pen settings")]
    [Header("Bounds")]
    [Tooltip("X = positive, Y = negative")]
    public Vector2 zBounds;
    [Tooltip("X = positive, Y = negative")]
    public Vector2 xBounds;

    [Header("Pen data")]
    public FoodContainer foodContainer = null;
    public ShitBucket shitBucket = null;
    
    [Header("Debug")]
    [SerializeField] private bool drawBounds = false;

    private void Update() {
        if (drawBounds)
            DrawBounds();
    }

    private void DrawBounds() {
        Debug.DrawLine(new Vector3(xBounds[0], 1, zBounds[0]), new Vector3(xBounds[0], 1, zBounds[1]));
        Debug.DrawLine(new Vector3(xBounds[0], 1, zBounds[1]), new Vector3(xBounds[1], 1, zBounds[1]));
        Debug.DrawLine(new Vector3(xBounds[1], 1, zBounds[1]), new Vector3(xBounds[1], 1, zBounds[0]));
        Debug.DrawLine(new Vector3(xBounds[1], 1, zBounds[0]), new Vector3(xBounds[0], 1, zBounds[0]));
    }

    public Vector3 GetNewWalkPos(float yPos) {
        return new Vector3(Random.Range(xBounds[1], xBounds[0]), yPos, Random.Range(zBounds[1], zBounds[0]));
    }

}
