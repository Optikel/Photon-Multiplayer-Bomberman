using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehaviour : MonoBehaviour
{
    public GameObject BombMesh;
    public GameObject Explosion;
    public int Power = 3; //Distance from origin, meaning power 3 is 2 + 1 + 3 = 5 tiles
    public float Timer = 3f;
    public float RateOfBlink = 1f;

    private float CountDown = 0f;
    private float BlinkingTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CountDown += Time.deltaTime;

        //Hide bomb Mesh
        if(CountDown >= Timer)
        {
            BombMesh.SetActive(false);
        }

        //Destroy Object
        if (CountDown >= Timer + 1)
        {
            Debug.Log("COuntdown:" + CountDown + "| Timer: " + Timer + 1);
            Object.Destroy(BombMesh.GetComponentInParent<Transform>().gameObject);
        }

        Blink(RateOfBlink, Color.black, Color.red);
    }

    void Blink(float intervals, Color originalColor, Color blinkColor)
    {
        BlinkingTimer += Time.deltaTime;

        Material mat = BombMesh.GetComponent<MeshRenderer>().material;
        if(BlinkingTimer > RateOfBlink * 0.5f)
        {
            mat.SetColor("_Color", mat.GetColor("_Color") == originalColor ? blinkColor : originalColor);
            BlinkingTimer = 0;
        }
    }
}
