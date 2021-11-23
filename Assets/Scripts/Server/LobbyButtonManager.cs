using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class LobbyButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CreateButton;
    [SerializeField]
    private GameObject JoinButton;
    [SerializeField]
    private GameObject NickName;

    [SerializeField]
    private GameObject CurrentScene;
    [SerializeField]
    private GameObject CreateScene;
    [SerializeField]
    private GameObject JoinScene;



    string m_NickName;
    // Start is called before the first frame update
    void Start()
    {
        CreateButton.GetComponent<Button>().onClick.AddListener(OnClickCreate);
        CreateButton.GetComponent<Button>().onClick.AddListener(JoinLobby);

        JoinButton.GetComponent<Button>().onClick.AddListener(OnClickJoin);
        JoinButton.GetComponent<Button>().onClick.AddListener(JoinLobby);
    }

    void JoinLobby()
    {
        if (m_NickName.Length > 0)
        {
            PhotonNetwork.NickName = m_NickName;
            Debug.Log("NickName - " + PhotonNetwork.NickName);

            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();

            //Debug.Log("Joined Lobby - " + PhotonNetwork.CurrentLobby);
        }
    }

    void OnClickCreate()
    {
        if (NickName.GetComponent<Text>().text.Length == 0)
        {
            m_NickName = "Player" + Random.Range(0, 10000);
        }
        else
        {
            m_NickName = NickName.GetComponent<Text>().text;
        }

        CurrentScene.SetActive(false);
        CreateScene.SetActive(true);
    }

    void OnClickJoin()
    {
        if (NickName.GetComponent<Text>().text.Length == 0)
        {
            m_NickName = "Player" + Random.Range(0, 10000);
        }
        else
        {
            m_NickName = NickName.GetComponent<Text>().text;
        }

        CurrentScene.SetActive(false);
        JoinScene.SetActive(true);
    }
}
