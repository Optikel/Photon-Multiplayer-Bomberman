using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BombBehaviour : MonoBehaviour
{
    public GameObject BombMesh;
    public GameObject Explosion;
    public GameObject[] Indestructables;
    public int Power = 3; //Distance from origin, meaning power 3 is 2 + 1 + 3 = 5 tiles
    public float Timer = 3f;
    public float RateOfBlink = 1f;

    private float CountDown = 0f;
    private float BlinkingTimer = 0f;
    private BombState m_State = BombState.Blink;
    private bool m_Exploded = false;
    enum BombState
    {
        Blink,
        Explode
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CountDown += Time.deltaTime;
        Blink(RateOfBlink, Color.black, Color.red);

        switch (m_State)
        {
            case BombState.Blink:
            {
                //Hide bomb Mesh
                if (CountDown >= Timer)
                {
                    BombMesh.SetActive(false);
                    CountDown = 0;
                    m_State = BombState.Explode;
                }
                break;
            }
            case BombState.Explode:
            {
                //Destroy Object
                if (CountDown >= 1)
                {
                    CountDown = 0;
                    Object.Destroy(BombMesh.GetComponentInParent<Transform>().gameObject);
                }
                if(!m_Exploded)
                    CreateExplosion(Explosion, Power);
                break;
            }
            default:
                break;
        }
    }

    void Blink(float intervals, Color originalColor, Color blinkColor)
    {
        BlinkingTimer += Time.deltaTime;

        Material mat = BombMesh.GetComponent<MeshRenderer>().material;
        if(BlinkingTimer > intervals * 0.5f)
        {
            mat.SetColor("_Color", mat.GetColor("_Color") == originalColor ? blinkColor : originalColor);
            BlinkingTimer = 0;
        }
    }

    bool CanSpawn(Vector3 Origin, Vector3 Direction, float Magnitude)
    {
        //True if no collide, false if collide
        bool rayCollide = Physics.Raycast(Origin, Direction, Magnitude);

        //return !rayCollide && insideDestructables;
        return true;    
    }
    GameObject InstantiateExplosion(GameObject ExplosionPrefab, Vector3 position, Transform Parent)
    {
        Vector3 parentPosition = Parent.position;
        if(CanSpawn(parentPosition, position.normalized, position.magnitude))
        {
            GameObject obj = Instantiate(ExplosionPrefab, Parent);
            obj.GetComponent<Transform>().localPosition = position;
        }

        return null;
    }
    void CreateExplosion(GameObject ExplosionPreFab, float Power = 1)
    {
        m_Exploded = true;
        Transform centerPosition = BombMesh.GetComponent<Transform>().parent;
        //Spawn center
        Instantiate(ExplosionPreFab, centerPosition);
        for (int i = 1; i < Power; i++)
        {
            InstantiateExplosion(ExplosionPreFab, new Vector3(-i, 0, 0), centerPosition);           
            InstantiateExplosion(ExplosionPreFab, new Vector3(i, 0, 0), centerPosition);           
            InstantiateExplosion(ExplosionPreFab, new Vector3(0, 0, -i), centerPosition);           
            InstantiateExplosion(ExplosionPreFab, new Vector3(0, 0, i), centerPosition);           
        }
    }
}
