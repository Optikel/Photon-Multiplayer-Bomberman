using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text _roomName;
    [SerializeField]
    private Text _roomSize;

    public RoomInfo RoomInfo { get; private set; }
    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomName.text = roomInfo.Name;
        _roomSize.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
    }
}
