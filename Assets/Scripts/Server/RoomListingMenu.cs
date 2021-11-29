using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RectTransform _content;
    [SerializeField]
    private RoomListing _roomListing;

    [SerializeField]
    public RoomListing activeListing = null;

    public List<RoomListing> _listings = new List<RoomListing>();
    #region PUNCALLBACK
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index != -1)
                {
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                }
            }
            else
            {
                RoomListing listing = Instantiate(_roomListing, _content);
                if (listing != null)
                    listing.SetRoomInfo(info);
                _listings.Add(listing);
            }
        }
    }
    #endregion
    public void DeleteRoomList()
    {
        for(int index = 0; index < _content.childCount; index++)
        {
            Object.Destroy(_content.GetChild(index).gameObject);
        }
    }

}
