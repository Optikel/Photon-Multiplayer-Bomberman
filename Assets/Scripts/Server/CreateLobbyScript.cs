using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CreateLobbyScript : MonoBehaviourPunCallbacks
{
    [Header("Lobby Creation")]
    [SerializeField]
    private GameObject CreateButton;
    [SerializeField]
    private GameObject LobbyNameText;
    [SerializeField]
    private GameObject CurrentContext;
    [SerializeField]
    private GameObject NextContext;

    string m_LobbyName;
    bool m_ToLobby = false;
    // Start is called before the first frame update
    void Start()
    {
        CreateButton.GetComponent<Button>().onClick.AddListener(delegate { CreateLobby(); });
        CreateButton.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        m_LobbyName = LobbyNameText.GetComponent<Text>().text;
    }

    void CreateLobby()
    {
        if (m_LobbyName.Length > 0)
        {
            Debug.Log("Created Lobby - " + m_LobbyName);
            PhotonNetwork.CreateRoom(m_LobbyName);
            Debug.Log(this.GetComponentInParent<Transform>().name);
            m_ToLobby = true;
        }
    }

    void OnClick()
    {
        if(m_ToLobby)
        {
            CurrentContext.SetActive(false);
            NextContext.SetActive(true);

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }
    }

    #region Photon
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    #endregion
}
