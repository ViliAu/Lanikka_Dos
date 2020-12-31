using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testpaun : MonoBehaviour {
    public Entity[] entities;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(entities[0]);
        Instantiate(entities[1]);
    }

}
