using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

using HashTable = ExitGames.Client.Photon.Hashtable;
public class PlayerSpawn : MonoBehaviourPun
{
    public GameObject PlayerSpawnpoints;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Player me = PhotonNetwork.LocalPlayer;
            Debug.Log(me.ActorNumber);
            Room currentRoom = PhotonNetwork.CurrentRoom;
            GameObject go = PhotonNetwork.Instantiate("Player", PlayerSpawnpoints.GetComponentsInChildren<Transform>()[me.ActorNumber].position, Quaternion.identity);

            CinemachineVirtualCamera vCam = FindObjectOfType<CinemachineVirtualCamera>();
            vCam.Follow = go.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
