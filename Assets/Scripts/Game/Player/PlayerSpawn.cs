using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using HashTable = ExitGames.Client.Photon.Hashtable;
public class PlayerSpawn : MonoBehaviourPunCallbacks
{
    public GameObject PlayerSpawnpoints;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log(PlayerSpawnpoints.GetComponentsInChildren<Transform>());
            //Get Local Player
            Player me = PhotonNetwork.LocalPlayer;
            Debug.Log("Actor No. = " + me.ActorNumber);
            //Instantiate Player over PhotonNetwork
            PhotonNetwork.Instantiate("Player", PlayerSpawnpoints.GetComponentsInChildren<Transform>()[me.ActorNumber].position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
