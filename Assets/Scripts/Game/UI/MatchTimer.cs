using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
public class MatchTimer : MonoBehaviourPun
{
    public Text UI_MatchTimer;

    public Coroutine TimerCoroutine;
    [SerializeField]
    double MatchDuration = 10;
    private void OnEnable()
    {
        UI_MatchTimer.gameObject.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("StartMatchTime", RpcTarget.AllBuffered, PhotonNetwork.Time);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator StartTimerCountDown(double startTime)
    {
        while((PhotonNetwork.Time - startTime) < MatchDuration)
        {
            UpdateUITimer(MatchDuration - (PhotonNetwork.Time - startTime));
            yield return null;
        }

        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("EndGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void StartMatchTime(double startTime)
    {
        TimerCoroutine = StartCoroutine(StartTimerCountDown(startTime));
    }

    void UpdateUITimer(double time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);

        UI_MatchTimer.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
    }
}
