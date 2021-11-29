using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RectTransform _list;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();

    public override void OnJoinedRoom()
    {
        int index = 1;
        foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            PlayerListing listing = Instantiate(_playerListing, _list);
            if (listing != null)
            {
                listing.SetPlayerInfo(player, index);
                _listings.Add(listing);
                index++;
            }
        }
    }

    public override void OnLeftRoom()
    {
        foreach(PlayerListing listing in _listings)
        {
            Destroy(listing.gameObject);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player " + newPlayer.NickName + " joined " + PhotonNetwork.CurrentLobby.Name);
        PlayerListing newlisting = Instantiate(_playerListing, _list);
        if (newlisting != null)
        {
            newlisting.SetPlayerInfo(newPlayer, PhotonNetwork.CurrentRoom.PlayerCount);
            _listings.Add(newlisting);
        }
        UpdateIndex();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
        UpdateIndex();
    }


    void UpdateIndex()
    {
        int index = 1;
        foreach (PlayerListing listing in _listings)
        {
            listing.SetIndex(index++);
        }
    }
}
