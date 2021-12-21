using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInstantiation : MonoBehaviourPun
{
    public GameObject PlayerMesh;

    [Header("Stats")]
    const int MinPower = 0;
    const int MaxPower = 10;
    [SerializeField]
    [Range(MinPower, MaxPower)]
    public int PowerIncrease;

    const int MinBombCount = 1;
    const int MaxBombCount = 6;
    [SerializeField]
    [Range(MinBombCount, MaxBombCount)]
    public int BombCount;
    public int CurrentBombUsed = 0;

    const int MinSpeedMultiplier = 0;
    const int MaxSpeedMultiplier = 10;
    [SerializeField]
    [Range(MinSpeedMultiplier, MaxSpeedMultiplier)]
    public int SpeedMultiplier;

    [SerializeField]
    public bool CanPunch = false;

    [SerializeField]
    public bool Penetrative = false;
    // Start is called before the first frame update
    void Start()
    {
        //Putting player GO into a PlayerContainer 
        transform.parent = GameObject.Find("PlayerContainer").transform;

        Material newMaterial = new Material(PlayerMesh.GetComponent<MeshRenderer>().material);
        newMaterial.SetColor("_Color", GameManager.GetColor(photonView.OwnerActorNr - 1));
        PlayerMesh.GetComponent<MeshRenderer>().material = newMaterial;
        
        //Setting virtualCamera to look at newPlayer(belongs to you)
        if (photonView.IsMine)
        {
            CinemachineVirtualCamera vCam = FindObjectOfType<CinemachineVirtualCamera>();
            vCam.Follow = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanBomb()
    {
        return CurrentBombUsed < BombCount;
    }

    [PunRPC]
    void SetBombCount(int bomb)
    {
        BombCount = Mathf.Clamp(bomb, MinBombCount, MaxBombCount);
    }
    [PunRPC]
    void SetFirePower(int power)
    {
        PowerIncrease = Mathf.Clamp(power, MinPower, MaxPower);
    }

    [PunRPC]
    void SetSpeed(int speed)
    {
        SpeedMultiplier = Mathf.Clamp(speed, MinSpeedMultiplier, MaxSpeedMultiplier);
    }
    [PunRPC]
    void EnablePunch()
    {
        CanPunch = true;
    }

    [PunRPC]
    void EnablePenetration()
    {
        Penetrative = true;
    }
}
