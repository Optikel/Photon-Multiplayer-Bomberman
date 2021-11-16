using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawn : MonoBehaviour
{
    public GameObject BombObj;
    public GameObject BombContainer;
    public CharacterMovement movementScript;

    [Range(0, 10)]
    public int MaxBombs;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject obj;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float heightCompensation = GetComponent<BoxCollider>().size.y * 0.5f - BombObj.GetComponentInChildren<BoxCollider>().size.y * 0.5f;
            Vector3 v3_heightOffset = new Vector3(0, heightCompensation, 0);

            Vector3 position = movementScript.targetPosition - v3_heightOffset;
            bool available = true;
            foreach(Transform otherObj in BombContainer.GetComponentInChildren<Transform>())
            {
                if (position == otherObj.localPosition)
                {
                    available = false;
                }
            }

            if(available)
            {
                GameObject bomb = Instantiate(BombObj, position, Quaternion.identity, BombContainer.transform) as GameObject;
            }
            //bomb.GetComponent<Collider>().enabled = false;
        }

    }
}
