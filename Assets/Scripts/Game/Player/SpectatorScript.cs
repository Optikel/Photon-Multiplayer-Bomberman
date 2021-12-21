using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class SpectatorScript : MonoBehaviourPun
{
    public GameObject PlayerContainer;
    public CinemachineVirtualCamera VCamera;

    [SerializeField]
    private int SpectateIndex;
    [SerializeField]
    private bool Enabled;
    // Start is called before the first frame update
    void Start()
    {
        PlayerContainer = GameObject.Find("PlayerContainer");
        VCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
            return;

        ProcessInput();
        if (PlayerContainer.transform.childCount > 0)
        {
            VCamera.Follow = PlayerContainer.GetComponentsInChildren<PlayerInstantiation>()[SpectateIndex].transform;
        }
        else
        {
            VCamera.Follow = null;
        }
       
    }

    void ProcessInput()
    {
        object start;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameManager.ROOM_GAME_START, out start);
        if (!(bool)start)
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            SpectateIndex--;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SpectateIndex++;
        }

        //SpectateIndex = Mathf.Repeat(SpectateIndex, PlayerContainer.transform.childCount - 1);
        SpectateIndex = (int)Mathf.Repeat(SpectateIndex, PlayerContainer.transform.childCount);
    }
}
