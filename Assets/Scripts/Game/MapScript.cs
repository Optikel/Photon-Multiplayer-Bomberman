using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<GameManager>().Map = gameObject;
        transform.SetSiblingIndex(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
