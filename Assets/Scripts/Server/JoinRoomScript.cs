using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class JoinRoomScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button JoinButton;
    [SerializeField]   
    private Button BackButton;
    [SerializeField]
    public GameObject Context;

    void Start()
    {
        BackButton.onClick.AddListener(OnClickBack);
        JoinButton.onClick.AddListener(OnClickJoin);
    }

    void Update()
    {

    }
    void OnClickJoin()
    {
        if(Context.GetComponentInChildren<RoomListingMenu>().activeListing)
        {
            string roomName = Context.GetComponentInChildren<RoomListingMenu>().activeListing.RoomInfo.Name;
            Debug.Log("Joining Lobby...");
            PhotonNetwork.JoinRoom(roomName);
        }

    }

    void OnClickBack()
    {
        Debug.Log("Leaving Lobby");

        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
    }
    
}
