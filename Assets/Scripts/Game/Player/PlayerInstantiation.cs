using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInstantiation : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        //Putting player GO into a PlayerContainer 
        photonView.RPC("AttachToContainer", RpcTarget.All);

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

    [PunRPC]
    void AttachToContainer()
    {
        transform.parent = GameObject.Find("PlayerContainer").transform;
    }
}
