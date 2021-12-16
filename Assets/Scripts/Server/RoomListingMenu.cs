using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    public RectTransform _content;
    public RoomListing _roomListing;

    public RoomListing activeListing = null;

    public List<RoomListing> _listings = new List<RoomListing>();
    #region PUNCALLBACK
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (_listings.Count > 0)
            {
                Debug.Log(_listings.Count);
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                RoomListing listing = _listings[index];
                if (info.RemovedFromList)
                {
                    if (index != -1)
                    {
                        Destroy(listing.gameObject);
                        _listings.RemoveAt(index);
                    }
                }
                else
                {
                    listing.SetRoomInfo(info);
                }
            }
            else 
            {
                RoomListing newlisting = Instantiate(_roomListing, _content);
                if (newlisting != null)
                    newlisting.SetRoomInfo(info);
                _listings.Add(newlisting);
            }
        }
    }
    #endregion
    public void DeleteRoomList()
    {
        foreach(RoomListing listing in _listings)
        {
            Destroy(listing.gameObject);
        }
        _listings.Clear();
    }

}
