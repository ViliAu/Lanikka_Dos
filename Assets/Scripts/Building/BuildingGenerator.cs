using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour {

    public GameObject wallPrefab;
    public GameObject topPrefab;
    public GameObject botPrefab;

    public int gridSize = 4;
    public int unitSize = 4;
    public int buildingHeight = 20;


    public Vector2[] controlPoints = new Vector2[] {
        new Vector2(-8, 8),
        new Vector2(8, 8),
        new Vector2(8, -8),
        new Vector2(-8, -8)
    };

    public Vector2[] points = new Vector2[4];

    private Transform _root;
    private Transform Root {
        get {
            if (_root == null) {
                _root = transform.Find("Building");
                if (_root == null) {
                    _root = new GameObject("Building").transform;
                }
                _root.transform.SetParent(transform);
            }
            return _root;
        }
    }

    // Asset that contains default prefabs
    private BGDefaultAssets defaultAssets;

    public void LoadDefaultAssets() {
        defaultAssets = Resources.Load("ScriptableObjects/Building Gen/BGDefault Assets") as BGDefaultAssets;
        if (defaultAssets == null) {
            Debug.LogWarning("Could not find default assets. Maybe it was moved or renamed?");
        }
    }

    public void BuildingModified() {
        SnapPoints();
        ClearMeshes();
        SpawnWalls();
    }

    void SnapPoints() {
        controlPoints.CopyTo(points, 0);

        for (int i = 0; i < points.Length; i++) {
            points[i].x = Mathf.Round(points[i].x / gridSize) * gridSize;
            points[i].y = Mathf.Round(points[i].y / gridSize) * gridSize;
        }
    }

    void ClearMeshes() {
        int childCount = Root.childCount;
        for (int i = 0; i < childCount; i++) {
            DestroyImmediate(Root.GetChild(0).gameObject);
        }
    }

    void SpawnWalls() {
        int numStoreys = Mathf.RoundToInt(buildingHeight / unitSize);
        float storeyHeight = 0;
        for (int y = 0; y < numStoreys; y++) {
            int numWalls = 0;
            for (int point = 0; point < points.Length; point++) {
                int next = point == points.Length - 1 ? 0 : point + 1;
                Vector2 nextDir = (points[next] - points[point]).normalized;
                numWalls = Mathf.RoundToInt(Vector2.Distance(points[point], points[next]) / unitSize);

                // Spawn gameojbexts
                for (int x = 0; x < numWalls; x++) {
                    Vector2 spawnPos = points[point] + (nextDir * unitSize * x) + new Vector2(transform.position.x, transform.position.z);

                    // Spawn clone
                    GameObject prefab = GetWallObject(y, numStoreys);
                    if (prefab == null)
                        return;

                    GameObject clone = Instantiate(prefab, new Vector3(spawnPos.x, storeyHeight, spawnPos.y), Quaternion.identity);
                    // Orientate clone
                    clone.transform.forward = GetWallNormal(point);
                    clone.transform.position += clone.transform.right * unitSize;

                    // Set clone propreties
                    clone.transform.SetParent(Root);
                    clone.name = "Wall";
                }
            }

            storeyHeight += GetWallHeight(y, numWalls);
        }
    }

    Vector3 GetWallNormal(int pointIndex) {
        switch(pointIndex) {
            case 0:
            return new Vector3(0, 0, 1);
            case 1:
            return new Vector3(1, 0, 0);
            case 2:
            return new Vector3(0, 0, -1);
            default:
            return new Vector3(-1, 0, 0);
        }
    }

    float GetWallHeight(int y, int numWalls) {
        if (y == 0) 
            return 1;
        else 
            return unitSize;
    }

    GameObject GetWallObject(int y, int numWalls) {
        if (y == 0) {
            return botPrefab != null ? botPrefab : defaultAssets.botWallPrefab;
        }
        else if (y == numWalls - 1)
            return topPrefab != null ? topPrefab : defaultAssets.topWallPrefab;
        else
            return wallPrefab != null ? wallPrefab : defaultAssets.wallPrefab;
    }

    void OnDrawGizmos() {
        DrawLines();
    }

    void DrawLines() {
        /*
        Vector2[] points = controlPoints;
        float yOffset = 0.01f;

        for (int i = 0; i < points.Length; i++) {
            Vector3 posA = new Vector3(points[i].x, yOffset, points[i].y) + transform.position;
            Vector3 posB;
            
            if (i == points.Length - 1) {
                posB = new Vector3(points[0].x, yOffset, points[0].y) + transform.position;
            }
            else {
                posB = new Vector3(points[i + 1].x, yOffset, points[i + 1].y) + transform.position;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(posA, posB);

        }
        */
    }

    public void ResetControlPoints() {
        controlPoints = new Vector2[] {
            new Vector2(-8, 8),
            new Vector2(8, 8),
            new Vector2(8, -8),
            new Vector2(-8, -8)
        };
    }
}
