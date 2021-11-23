using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text ServerStatus;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
        Debug.Log("Connecting to server");


        ServerStatus.text = PhotonNetwork.Server.ToString();
    }
    private void Update()
    {
    }
    #region PUN CallBacks   
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the" + PhotonNetwork.CloudRegion + " server!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Server - " + cause);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }
    #endregion
}
