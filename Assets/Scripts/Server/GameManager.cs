using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    private string LobbySceneName = "DemoAsteroids-LobbyScene";
    #region PUN CALLBACKS

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(LobbySceneName);
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            // StartCoroutine(SpawnAsteroid());
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckEndOfGame();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
    }

    #endregion
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    #region GAME

    private void StartGame()
    {
        Debug.Log("StartGame!");

        Vector3 position = new Vector3(-9.8f, 0.0f, -3.2f);
        Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        GameObject player = PhotonNetwork.Instantiate("JohnLemon", position, rotation, 0);      // avoid this call on rejoin (ship was network instantiated before)

        //if (player.GetComponent<PhotonView>().IsMine)
        //{
        //    virtualCam.Follow = player.transform;
        //    virtualCam.LookAt = player.transform;
        //}

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    SpawnGhosts();
        //}
    }

    private void CheckEndOfGame()
    {
    }

    #endregion
}
