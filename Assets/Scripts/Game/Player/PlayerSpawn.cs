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
    public void Spawn()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            //Get Local Player
            Player me = PhotonNetwork.LocalPlayer;
            //Instantiate Player over PhotonNetwork
            PhotonNetwork.Instantiate("Player", PlayerSpawnpoints.GetComponentsInChildren<Transform>()[me.ActorNumber].position, Quaternion.identity);
        }
    }
}
