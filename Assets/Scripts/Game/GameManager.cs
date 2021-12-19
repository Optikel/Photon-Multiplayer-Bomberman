using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public CinemachineVirtualCamera VCamera;
    public GameObject PlayerContainer;
    public GameObject BombContainer;
    public Text UI_Winner; 

    public bool GameStarted;
    public const string PLAYER_ALIVE = "PlayerAlive";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public const string ROOM_GAME_START = "GameStart";
    public static Color GetColor(int colorChoice)
    {
        switch (colorChoice)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
        }

        return Color.black;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable roomprops = new Hashtable
            {
                {ROOM_GAME_START, false}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomprops);
        }

        Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, true},
                {PLAYER_ALIVE, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }

    [PunRPC]
    public void StartGame()
    {
        Debug.Log("Start Game!");

        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable roomprops = new Hashtable
            {
                {ROOM_GAME_START, true}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomprops);
        }
    }

    [PunRPC]
    public void EndGame()
    {
        Debug.Log("Game Ended");

        if (GetComponent<MatchTimer>().TimerCoroutine != null)
            StopCoroutine(GetComponent<MatchTimer>().TimerCoroutine);

        UI_Winner.gameObject.SetActive(true);
        UI_Winner.text = UI_Winner.text.Replace("*Name*", FindLastPlayerAlive().NickName);

        StartCoroutine(SendToLobby());
    }

    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        object gameStarted;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(ROOM_GAME_START, out gameStarted);

        if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
        {
            if (!(gameStarted == null ? false : (bool)gameStarted))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    GetComponent<PlayerSpawn>().Spawn();

                    if (PhotonNetwork.IsMasterClient)
                        photonView.RPC("StartCountDown", RpcTarget.AllBuffered, PhotonNetwork.Time);
                }
            }
        }
        if (changedProps.ContainsKey(PLAYER_ALIVE))
        {
            if (gameStarted == null ? false : (bool)gameStarted) //Make sure game has already started
            {
                if(CheckLastPlayerAlive())
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        Hashtable roomprops = new Hashtable
                        {
                            {ROOM_GAME_START, false}
                        };

                        PhotonNetwork.CurrentRoom.SetCustomProperties(roomprops);
                        photonView.RPC("EndGame", RpcTarget.AllBuffered);
                    }
                }
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey(ROOM_GAME_START))
        {
            GameStarted = (bool)propertiesThatChanged[ROOM_GAME_START];
        }
    }
    #endregion

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }

    private bool CheckLastPlayerAlive()
    {
        int AliveCount = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isAlive;

            if (p.CustomProperties.TryGetValue(PLAYER_ALIVE, out isAlive))
            {
                if ((bool)isAlive)
                {
                    AliveCount++;
                }
            }
        }

        if (AliveCount <= 1)
            return true;
        else
            return false;
    }

    private Player FindLastPlayerAlive()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.ContainsKey(PLAYER_ALIVE))
            {
                object isAlive = p.CustomProperties[PLAYER_ALIVE];
                if ((bool)isAlive) return p;
            }
        }

        return null;
    }

    IEnumerator SendToLobby()
    {
        yield return new WaitForSeconds(1); //TODO: Adjust Lobby Scripts to accept load lobby without having to disconnect
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.Disconnect();
        yield return new WaitForSeconds(1);
        PhotonNetwork.LoadLevel("Lobby");
    }
}
