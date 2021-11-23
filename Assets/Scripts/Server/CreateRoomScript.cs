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
    private GameObject CurrentScene;
    [SerializeField]
    private GameObject LobbyScene;
    [SerializeField]
    private GameObject SelectionScene;

    string m_RoomName;
    void Start()
    {
        CreateButton.GetComponent<Button>().onClick.AddListener(OnClickCreate);

        BackButton.GetComponent<Button>().onClick.AddListener(OnClickBack);
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
            CurrentScene.SetActive(false);
            LobbyScene.SetActive(true);
        }
    }
    void OnClickBack()
    {
        CurrentScene.SetActive(false);
        SelectionScene.SetActive(true);
    }

    #region PUNCALLBACKS

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room(" + m_RoomName + ") Successfully!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room - " + PhotonNetwork.CurrentRoom);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Created Failed - " + message);
    }
    #endregion
}
