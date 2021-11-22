using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class NetworkController : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the" + PhotonNetwork.CloudRegion + " server!");
    } 

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }
    // Update is called once per frame
    void Update()
    {
    }
}
