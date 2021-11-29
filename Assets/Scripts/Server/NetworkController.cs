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

    [SerializeField]
    private GameObject ActiveContext;
    [SerializeField]
    private LobbyButtonManager LobbyContext;
    [SerializeField]
    private CreateRoomScript LobbyCreateContext;
    [SerializeField]
    private JoinRoomScript LobbyJoinContext;
    [SerializeField]
    private RoomButtonManager RoomContext;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to server...");
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
        ServerStatus.text = "Connecting...";
    }

    #region PUN CallBacks
    public override void OnConnectedToMaster()
    {
        ServerStatus.text = PhotonNetwork.ServerAddress;
        Debug.Log("Connected to the" + PhotonNetwork.CloudRegion + " server!");

        ActiveContext = LobbyContext.Context;
        ActiveContext.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Server - " + cause);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby - " + PhotonNetwork.CurrentLobby);
        ActiveContext.SetActive(false);
        ActiveContext = LobbyContext.ToCreate ? LobbyCreateContext.Context: LobbyJoinContext.Context;
        ActiveContext.SetActive(true);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Leaving Lobby");
        ActiveContext.SetActive(false);
        ActiveContext = LobbyContext.Context;
        ActiveContext.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room - " + PhotonNetwork.CurrentRoom.Name);
        ActiveContext.SetActive(false);
        ActiveContext = RoomContext.Context;
        ActiveContext.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Leave Room");
        ActiveContext.SetActive(false);
        ActiveContext = LobbyContext.Context;
        ActiveContext.SetActive(true);
    }
    #endregion
}
