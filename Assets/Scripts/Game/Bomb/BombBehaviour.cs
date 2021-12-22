using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Photon.Pun;

public class BombBehaviour : MonoBehaviourPun
{
    int BaseFirePower = 1;
    [Header("Prefabs")]
    public GameObject BombMesh;
    public GameObject Explosion;

    [Header("Timing")]
    public float PrimingTime = 3f;
    public float LingerTime = 1f;
    public float RateOfBlink = 1f;

    public Vector3 Velocity = Vector3.zero;
    [Header("Debug")]
    public bool deleteAfterExplosion = true;

    //For Exploding 
    private BombState State = BombState.Blink;
    private float CountDown = 0f;
    private float BlinkingTimer = 0f;
    private bool m_Exploded = false;
    private Color BaseColor;
    //Properties
    int Power = 0;

    [SerializeField]
    public bool Penetrative = false;

    public enum BombState
    {
        Blink,
        Explode
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = GameObject.Find("BombContainer").transform;
        BaseColor = BombMesh.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        CountDown += Time.deltaTime;
        Blink(RateOfBlink, BaseColor, Color.red);

        switch (State)
        {
            case BombState.Blink:
                {
                    //Hide bomb Mesh
                    if (CountDown >= PrimingTime)
                    {
                        BombMesh.SetActive(false);
                        CountDown = 0;
                        State = BombState.Explode;
                    }
                    break;
                }
            case BombState.Explode:
                {
                    if (!m_Exploded)
                    {
                        m_Exploded = true;
                        if (photonView.IsMine)
                        {
                            photonView.RPC("CreateExplosion", RpcTarget.All, photonView.ViewID);
                        }
                    }
                    //Destroy Object
                    else if (CountDown >= LingerTime)
                    {
                        CountDown = 0;
                        if(deleteAfterExplosion)
                        {
                            if(photonView.IsMine)
                            {
                                PhotonNetwork.Destroy(photonView);
                            }
                        }
                    }
                   
                    break;
                }
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, Velocity.normalized);
        if (Physics.Raycast(ray, 0.6f, LayerMask.GetMask("Destructable") | LayerMask.GetMask("Indestructable") | LayerMask.GetMask("Player") | LayerMask.GetMask("Bomb")))
        {
            transform.position = RoundVector3(transform.position);
            Velocity = Vector3.zero;
        }
        transform.position += Velocity * Time.fixedDeltaTime;
    }

    Vector3 RoundVector3 (Vector3 target)
    {
        return new Vector3(Mathf.RoundToInt(target.x), Mathf.RoundToInt(target.y), Mathf.RoundToInt(target.z));
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
    void CreateExplosion(int viewID)
    {
        CharacterController[] arr = FindObjectsOfType<CharacterController>();

        foreach (CharacterController character in arr)
        {
            if (character.photonView.Owner == PhotonView.Find(viewID).Owner)
            {
                character.GetComponent<PlayerInstantiation>().CurrentBombUsed--;
                Power = BaseFirePower + character.GetComponent<PlayerInstantiation>().PowerIncrease;
            }
        }

        if (!photonView.IsMine)
            return;

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
        for (int i = 1; i <= Power; i++)
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
    public void SetState(BombState state)
    {
        State = state;
        CountDown = 0;
    }
    public void ChainExplode()
    {
        if(State != BombState.Explode)
            CountDown = PrimingTime - 0.1f;
    }
}
