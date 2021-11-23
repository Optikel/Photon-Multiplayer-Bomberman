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
    private GameObject CurrentScene;
    [SerializeField]
    private GameObject LobbyScene;
    [SerializeField]
    private GameObject SelectionScene;
    void Start()
    {
        JoinButton.onClick.AddListener(OnClickJoin);
        BackButton.onClick.AddListener(OnClickBack);
    }

    void OnClickJoin()
    {
        CurrentScene.SetActive(false);
        LobbyScene.SetActive(true);
    }
    void OnClickBack()
    {
        Debug.Log("Leaving Lobby");

        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

        CurrentScene.SetActive(false);
        SelectionScene.SetActive(true);
    }
}
