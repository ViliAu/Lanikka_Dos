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

    private void OnDrawGizmos() {
        if (drawBounds)
            DrawBounds();
    }

    private void DrawBounds() {
        Gizmos.color = Color.red;
        float yPos = 0.05f;
        Gizmos.DrawLine(new Vector3(xBounds[0], yPos, zBounds[0]), new Vector3(xBounds[0], yPos, zBounds[1]));
        Gizmos.DrawLine(new Vector3(xBounds[0], yPos, zBounds[1]), new Vector3(xBounds[1], yPos, zBounds[1]));
        Gizmos.DrawLine(new Vector3(xBounds[1], yPos, zBounds[1]), new Vector3(xBounds[1], yPos, zBounds[0]));
        Gizmos.DrawLine(new Vector3(xBounds[1], yPos, zBounds[0]), new Vector3(xBounds[0], yPos, zBounds[0]));
    }

    public Vector3 GetNewWalkPos(float yPos) {
        return new Vector3(Random.Range(xBounds[1], xBounds[0]), yPos, Random.Range(zBounds[1], zBounds[0]));
    }

}
