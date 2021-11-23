using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
public class RoomButtonManager : MonoBehaviour
{
    [SerializeField]
    private Button BackButton;
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private GameObject CurrentScene;
    [SerializeField]
    private GameObject JoinScene;

    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(OnClickBack);
        StartButton.onClick.AddListener(OnClickStart);
    }

    void OnClickStart()
    {

    }
    void OnClickBack()
    {
        PhotonNetwork.LeaveRoom();
        CurrentScene.SetActive(false);
        JoinScene.SetActive(true);
    }
}
