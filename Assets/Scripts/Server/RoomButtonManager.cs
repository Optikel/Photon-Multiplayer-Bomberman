using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
public class RoomButtonManager : MonoBehaviour
{
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button BackButton;
    [SerializeField]
    public GameObject Context;
    // Start is called before the first frame update
    void Start()
    {
        BackButton.onClick.AddListener(OnClickBack);
        StartButton.onClick.AddListener(OnClickStart);
    }

    void OnClickStart()
    {
        Player localplayer = PhotonNetwork.LocalPlayer;
        if (localplayer.CustomProperties.ContainsKey("Ready"))
        {
            Hashtable properties = new Hashtable();
            properties.Add("Ready", !(bool)PhotonNetwork.LocalPlayer.CustomProperties["Ready"]);
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }
    }

    void OnClickBack()
    {
        Debug.Log("Leaving Room...");
        PhotonNetwork.LeaveRoom();
    }
}
