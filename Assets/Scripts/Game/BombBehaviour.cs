using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Photon.Pun;

public class BombBehaviour : MonoBehaviourPun
{
    [Header("Prefabs")]
    public GameObject BombMesh;
    public GameObject Explosion;
    
    [Header("Properties")]
    public int Power = 3; //Distance from origin, meaning power 3 is 2 + 1 + 3 = 5 tiles
    public bool Penetrative = false;
   
    [Header("Timing")]
    public float Timer = 3f;
    public float RateOfBlink = 1f;

    [Header("Debug")]
    public bool deleteAfterExplosion = true;

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
                        if(deleteAfterExplosion)
                        {
                            if(photonView.IsMine)
                                PhotonNetwork.Destroy(photonView);
                        }
                    }
                    if (!m_Exploded)
                    {
                        m_Exploded = true;
                        if(photonView.IsMine)
                            photonView.RPC("CreateExplosion", RpcTarget.All, photonView.ViewID, Power);
                    }
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
        if (BlinkingTimer > intervals * 0.5f)
        {
            mat.SetColor("_Color", mat.GetColor("_Color") == originalColor ? blinkColor : originalColor);
            BlinkingTimer = 0;
        }
    }

    [PunRPC]
    void CreateExplosion(int viewID, int Power = 1)
    {
        if (!photonView.IsMine)
            return;

        Debug.Log("Create Explosion by " + PhotonNetwork.LocalPlayer);
        //Spawn center
        GameObject obj = PhotonNetwork.Instantiate(Explosion.name, transform.position, Quaternion.identity);
        obj.GetPhotonView().RPC("AttachToBomb", RpcTarget.All, viewID);
        
        InstantiateExplosion(new Vector3(-1, 0,  0), viewID);
        InstantiateExplosion(new Vector3( 1, 0,  0), viewID);
        InstantiateExplosion(new Vector3( 0, 0, -1), viewID);
        InstantiateExplosion(new Vector3( 0, 0,  1), viewID);
    }
    void InstantiateExplosion(Vector3 direction, int viewId)
    {
        bool bStop = false;
        for (int i = 1; i < Power; i++)
        {
            if (bStop)
                break;

            Vector3 parentPosition = transform.position;
            if (CanSpawn(parentPosition, direction.normalized, i))
            {
                GameObject obj = PhotonNetwork.Instantiate(Explosion.name, parentPosition + direction * i, Quaternion.identity);
                obj.GetPhotonView().RPC("AttachToBomb", RpcTarget.All, viewId);

                if (!Penetrative)
                {
                    RaycastHit hit;
                    Ray ray = new Ray(parentPosition, direction.normalized);
                    if (Physics.Raycast(ray, out hit, i, LayerMask.GetMask("Destructable")))
                    {
                        bStop = true;
                    }
                }
            }
        }
    }

    bool CanSpawn(Vector3 Origin, Vector3 Direction, float Magnitude)
    {
        //True if no collide, false if collide
        bool rayCollide = Physics.Raycast(Origin, Direction, Magnitude, LayerMask.GetMask("Indestructable"));
        return !rayCollide;
    }

    [PunRPC]
    void AttachToContainer()
    {
        transform.parent = GameObject.Find("BombContainer").transform;
    }
}
