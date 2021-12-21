using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


using Hashtable = ExitGames.Client.Photon.Hashtable;
public class NetworkController : MonoBehaviourPunCallbacks
{
    public Text ServerStatus;
    public GameObject PreviousPanel;
    public GameObject ActivePanel;
    [Header("Login Panel")]
    public GameObject LobbyPanel;
    public Text PlayerPlaceHolder;
    public Text PlayerNickname;

    [Header("Create Panel")]
    public GameObject CreateRoomPanel;
    public Text RoomNameInput;

    [Header("Join Panel")]
    public GameObject JoinRoomPanel;
    [Header("Room Panel")]
    public GameObject RoomPanel;
    public Text RoomName;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        ServerStatus.text = ServerStatus.text.Replace("*Server Status*", "Connecting to server...");

        PhotonNetwork.GameVersion = "0.0.1";

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings(); //Connects to Photon master servers
        else
        {
            ServerStatus.text = ServerStatus.text.Replace("Connecting to server...", PhotonNetwork.ServerAddress);

            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
                OnJoinedRoom();
            }
            else if (PhotonNetwork.InLobby) OnJoinedLobby();
        }
    }

    #region PUN CallBacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to " + PhotonNetwork.CloudRegion + " server!");
        ServerStatus.text = ServerStatus.text.Replace("Connecting to server...", PhotonNetwork.ServerAddress);

        PlayerPlaceHolder.text = "Player" + Random.Range(0, 10000);
        PhotonNetwork.NickName = PlayerPlaceHolder.text;

        SetActivePanel(LobbyPanel);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from Server - " + cause);
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = PlayerNickname.text.Length > 0 ? PlayerNickname.text : PlayerPlaceHolder.text;
        Debug.Log(PhotonNetwork.NickName + " joined Lobby - " + PhotonNetwork.CurrentLobby.Name);

        ActivePanel.GetComponentInChildren<RoomListingMenu>().activeListing = null;
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Leaving Lobby");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room(" + PhotonNetwork.CurrentRoom.Name + ") Successfully!");
    }

    public override void OnJoinedRoom()
    {
        if(ActivePanel == JoinRoomPanel)
            ActivePanel.GetComponentInChildren<RoomListingMenu>().DeleteRoomList();

        SetActivePanel(RoomPanel);
        
        Debug.Log(PhotonNetwork.NickName + " joined Room(" + PhotonNetwork.CurrentRoom.Name + ") Successfully!");
        RoomName.text = RoomName.text.Replace("*Room Name*", PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        ActivePanel.GetComponentInChildren<PlayerListingMenu>().DestroyList();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Created Failed - " + message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ActivePanel.GetComponentInChildren<PlayerListingMenu>().InsertList(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ActivePanel.GetComponentInChildren<PlayerListingMenu>().RemoveFromList(otherPlayer);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        PlayerListingMenu menu = ActivePanel.GetComponentInChildren<PlayerListingMenu>();

        foreach (PlayerListing listing in menu._listings)
        {
            if (listing.Player == targetPlayer)
            {
                if (changedProps.ContainsKey("Ready"))
                {
                    listing.SetReadyText((bool)changedProps["Ready"] ? "Ready" : "Not Ready");
                }
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
    }

    #endregion

    #region UI CALLBACKS
    public void OnRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby) PhotonNetwork.JoinLobby();

        SetActivePanel(JoinRoomPanel);
    }

    public void OnLeaveRoomListButtonClicked()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();

        ActivePanel.GetComponentInChildren<RoomListingMenu>().DeleteRoomList();
        SetActivePanel(LobbyPanel);
    }

    public void OnJoinRoomButtonClicked()
    {
        RoomListing listing = ActivePanel.GetComponentInChildren<RoomListingMenu>().activeListing;
        if(listing)
            PhotonNetwork.JoinRoom(listing._roomName.text);
    }

    public void OnCreateRoomButtonClicked()
    {
        string roomName = RoomNameInput.text.Length > 0 ? RoomNameInput.text : "Room " + Random.Range(0, 10000);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = true;
        options.DeleteNullProperties = true;
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
    }

    public void OnLeaveRoomButtonClicked()
    {
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        RoomName.text = "Room Name: *Room Name*";
    }

    public void OnStartGameButtonClicked()
    {
        Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        object isPlayerReady;
        if (props.TryGetValue("Ready", out isPlayerReady))
        {
            if ((bool)isPlayerReady)
            {
                if(PhotonNetwork.IsMasterClient && ActivePanel.GetComponentInChildren<PlayerListingMenu>().CheckAllReady())
                {
                    Debug.Log("Starting game...");
                    PhotonNetwork.CurrentRoom.IsOpen = false;
                    PhotonNetwork.CurrentRoom.IsVisible = false;
                    PhotonNetwork.DestroyAll();
                    PhotonNetwork.LoadLevel("MainScene");
                }
                else
                {
                    props["Ready"] = !(bool)isPlayerReady;
                    PhotonNetwork.SetPlayerCustomProperties(props);
                }
            }
            else
            {
                props["Ready"] = !(bool)isPlayerReady;
                PhotonNetwork.SetPlayerCustomProperties(props);
            }
        }

       
    }

    #endregion
    public void SetActivePanel(GameObject newScene)
    {
        Debug.Log("Changing from " + (ActivePanel ? ActivePanel.name : "NULL") + " to " + newScene.name);

        if (ActivePanel)
        {
            ActivePanel.SetActive(false);
            PreviousPanel = ActivePanel;
        }

        ActivePanel = newScene;
        ActivePanel.SetActive(true);
    }
}
