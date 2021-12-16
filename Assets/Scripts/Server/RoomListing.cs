using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
public class RoomListing : MonoBehaviour, IPointerClickHandler
{
    public Text _roomName;
    public Text _roomSize;

    public RoomListingMenu menu;

    public bool isSelected = false;
    public RoomInfo RoomInfo { get; private set; }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomName.text = roomInfo.Name;
        _roomSize.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;

        menu = GetComponentInParent<RoomListingMenu>();
        GetComponent<Button>().onClick.AddListener(SetSelecionContext);
    }

    public void SetSelecionContext()
    {
        menu.activeListing = this;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 1)
        {
            if(menu.activeListing)
            {
                ColorBlock btncolor = menu.activeListing.GetComponent<Button>().colors;
                btncolor.normalColor = Color.white;
                menu.activeListing.GetComponent<Button>().colors = btncolor;
            }
            menu.activeListing = this;
            
            ColorBlock btncolor1 = menu.activeListing.GetComponent<Button>().colors;
            btncolor1.normalColor = Color.grey;
            menu.activeListing.GetComponent<Button>().colors = btncolor1;
        }
        if (eventData.clickCount == 2)
        {
            PhotonNetwork.JoinRoom(_roomName.text);
        }
    }

}