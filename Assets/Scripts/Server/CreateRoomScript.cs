using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
public class CreateRoomScript : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject CreateButton;
    [SerializeField]
    private GameObject BackButton;
    [SerializeField]
    private GameObject RoomName;
    [SerializeField]
    public GameObject Context;

    string m_RoomName;
    void Start()
    {
        CreateButton.GetComponent<Button>().onClick.AddListener(OnClickCreate);
        BackButton.GetComponent<Button>().onClick.AddListener(OnClickLeave);
    }

    void Update()
    {
        m_RoomName = RoomName.GetComponent<Text>().text;
    }

    void OnClickCreate()
    {
        if(m_RoomName.Length > 0)
        {
            Debug.Log("Creating Room");

            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;
            options.IsVisible = true;

            PhotonNetwork.CreateRoom(m_RoomName, options, TypedLobby.Default);
        }
    }

    void OnClickLeave()
    {
        PhotonNetwork.LeaveLobby();
    }
}
