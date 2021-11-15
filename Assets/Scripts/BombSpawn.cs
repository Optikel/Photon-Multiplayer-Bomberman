using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpawn : MonoBehaviour
{
    [Range(1f, 10f)]
    public float Distance = 1f;
    public GameObject BombPrefab;
    public CharacterMovement movementScript;

    private Transform characterTransform;
    // Start is called before the first frame update
    void Start()
    {
        characterTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Distance = Mathf.Clamp(Distance, 1f, 5f);
        //GameObject obj;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 dirVector = Quaternion.Euler(0, movementScript.targetAngle, 0) * Vector3.forward;

            GameObject bomb = Instantiate(BombPrefab, characterTransform.localPosition + dirVector * Distance, Quaternion.identity) as GameObject;
        }

        

    }
}
