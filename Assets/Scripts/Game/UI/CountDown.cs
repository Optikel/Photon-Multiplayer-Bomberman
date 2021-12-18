using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;

public class CountDown : MonoBehaviourPunCallbacks
{
    public Text UI_CountDown;

    int iCountDown = 3;
    // Start is called before the first frame update
    void Start()
    {
        if(CheckAllPlayerLoadedLevel())
            photonView.RPC("StartCountDown", RpcTarget.AllBuffered, PhotonNetwork.Time);
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator StartTimerCountDown(double startTime)
    {
        while ((PhotonNetwork.Time - startTime) < iCountDown)
        {
            UpdateUITimer(iCountDown - (PhotonNetwork.Time - startTime));
            yield return null;
        }

        UI_CountDown.gameObject.SetActive(false);
        GetComponent<MatchTimer>().enabled = true;
    }

    [PunRPC]
    void StartCountDown(double startTime)
    {
        StartCoroutine(StartTimerCountDown(startTime));
    }

    void UpdateUITimer(double time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);

        UI_CountDown.text = string.Format("{0:D1}", t.Seconds);
    }

    private bool CheckAllPlayerLoadedLevel()
    {
        //foreach (Player p in PhotonNetwork.PlayerList)
        //{
        //    object playerLoadedLevel;
        //    p
        //    if (p.CustomProperties.TryGetValue(JLGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
        //    {
        //        if ((bool)playerLoadedLevel)
        //        {
        //            continue;
        //        }
        //    }

        //    return false;
        //}

        return true;
    }
}
