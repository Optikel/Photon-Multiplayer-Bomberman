using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class LobbyButtonManager : MonoBehaviour
{
    [SerializeField]
    private Button CreateButton;
    [SerializeField]
    private Button JoinButton;
    [SerializeField]
    private Text NickName;
    [SerializeField]
    public GameObject Context;

    [HideInInspector]
    public bool ToCreate = false;

    string m_NickName;
    // Start is called before the first frame update
    void Start()
    {
        CreateButton.GetComponent<Button>().onClick.AddListener(OnClickCreate);
        JoinButton.GetComponent<Button>().onClick.AddListener(OnClickJoin);
    }

    void JoinLobby()
    {
        if (m_NickName.Length > 0)
        {
            PhotonNetwork.NickName = m_NickName;
            Debug.Log("NickName - " + PhotonNetwork.NickName);

            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();
        }
    }

    public void OnClickCreate()
    {
        ToCreate = true;
        if (NickName.GetComponent<Text>().text.Length == 0)
        {
            m_NickName = "Player" + Random.Range(0, 10000);
        }
        else
        {
            m_NickName = NickName.GetComponent<Text>().text;
        }

        JoinLobby();
    }

    public void OnClickJoin()
    {
        ToCreate = false;
        if (NickName.GetComponent<Text>().text.Length == 0)
        {
            m_NickName = "Player" + Random.Range(0, 10000);
        }
        else
        {
            m_NickName = NickName.GetComponent<Text>().text;
        }

        JoinLobby();
    }
}
