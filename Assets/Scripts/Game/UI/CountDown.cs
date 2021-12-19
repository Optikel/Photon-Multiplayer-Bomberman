using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.Events;

public class CountDown : MonoBehaviourPunCallbacks
{
    public Text UI_CountDown;

    public UnityEvent CountDownFinishEvent;
    public Coroutine CountDownTimer;
    int iCountDown = 3;
   
    void Awake()
    {
        if (CountDownFinishEvent == null)
            CountDownFinishEvent = new UnityEvent();

        CountDownFinishEvent.AddListener(OnCountDownEnd);
    }
    public IEnumerator StartTimerCountDown(double startTime)
    {
        while ((PhotonNetwork.Time - startTime) < iCountDown)
        {
            UpdateUITimer(iCountDown - (PhotonNetwork.Time - startTime));
            yield return null;
        }

        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("StartGame", RpcTarget.All);
        CountDownFinishEvent.Invoke();
    }
            
    [PunRPC]
    void StartCountDown(double startTime)
    {
        CountDownTimer = StartCoroutine(StartTimerCountDown(startTime));
    }

    void UpdateUITimer(double time)
    {
        TimeSpan t = TimeSpan.FromSeconds(time);

        UI_CountDown.text = string.Format("{0:D1}", t.Seconds);
    }


    private void OnDestroy()
    {
        CountDownFinishEvent.RemoveListener(OnCountDownEnd);
    }
    void OnCountDownEnd()
    {
        photonView.RPC("OnCountDownEndRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void OnCountDownEndRPC()
    {
        GetComponent<CountDown>().UI_CountDown.gameObject.SetActive(false);
        GetComponent<MatchTimer>().enabled = true;
    }
}
