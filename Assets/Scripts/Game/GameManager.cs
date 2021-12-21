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
    public GameObject Map;
    
    public CinemachineVirtualCamera VCamera;
    public GameObject PlayerContainer;
    public GameObject BombContainer;
    public Text UI_Winner; 

    public bool GameStarted;
    public int ToLobbyTimer = 5;
    
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
        transform.SetSiblingIndex(99);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject("Map", Vector3.zero, Quaternion.identity);
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
        if (CheckNumPlayersAlive() == 1)
            UI_Winner.text = UI_Winner.text.Replace("*Name*", FindLastPlayerAlive().NickName);
        else 
            UI_Winner.text = "TIE \n WHY NO WINNER?!";

        StartCoroutine(SendToLobby());
    }

    #region Pun Callbacks
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        object gameStarted;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(ROOM_GAME_START, out gameStarted);

        if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
        {
            if (changedProps[PLAYER_LOADED_LEVEL] != null)
            {
                if (!(gameStarted == null ? false : (bool)gameStarted)) //done
                {
                    if (CheckAllPlayerLoadedLevel()) //Problem
                    {
                        GetComponent<PlayerSpawn>().Spawn();

                        if (PhotonNetwork.IsMasterClient)
                        {
                            Debug.Log("Starting CountdownTimerRPC");
                            photonView.RPC("StartCountDown", RpcTarget.All, PhotonNetwork.Time);
                        }
                    }
                }
            }
        }
        if (changedProps.ContainsKey(PLAYER_ALIVE))
        {
            if (changedProps[PLAYER_ALIVE] != null)
            {
                if (gameStarted == null ? false : (bool)gameStarted) //Make sure game has already started
                {
                    if (CheckNumPlayersAlive() <= 1)
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
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.ContainsKey(ROOM_GAME_START))
        {
            GameStarted = (bool)propertiesThatChanged[ROOM_GAME_START];
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 0)
        {
            if (otherPlayer.IsMasterClient)
            {
                Player newMaster = null;
                while (newMaster != null)
                {
                    int i = 0;
                    newMaster = PhotonNetwork.CurrentRoom.GetPlayer(++i);   
                }
                PhotonNetwork.SetMasterClient(newMaster);
            }
        }
           
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("New Master Client is now " + newMasterClient.NickName + " (" + newMasterClient.ActorNumber + ")");
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

    private int CheckNumPlayersAlive()
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

        return AliveCount;
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
        Debug.Log("Sending To Lobby...");
        yield return new WaitForSeconds(ToLobbyTimer);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable());

            Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, null},
                {PLAYER_ALIVE, null}
            };
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                p.SetCustomProperties(props);
            }

            PhotonNetwork.DestroyAll();
            PhotonNetwork.OpRemoveCompleteCache(); //Remove any buffered rpc or events
            PhotonNetwork.LoadLevel("Lobby");
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
