using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using System.Linq;
public class PlayerListingMenu : MonoBehaviourPunCallbacks
{
    public RectTransform _list;
    public PlayerListing _playerListing;

    public List<PlayerListing> _listings = new List<PlayerListing>();

    public override void OnEnable()
    {
        Debug.Log("Playerlist Enabled");
        CreateList();
    }
    public void CreateList()
    {
        int index = 1;

        foreach (Player player in PhotonNetwork.PlayerList)
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
    
    public void InsertList(Player newPlayer)
    {
        Debug.Log("Insert " + newPlayer.NickName);
        PlayerListing newlisting = Instantiate(_playerListing, _list);
        if (newlisting != null)
        {
            newlisting.SetPlayerInfo(newPlayer, PhotonNetwork.CurrentRoom.PlayerCount);
            _listings.Add(newlisting);
        }
        UpdatePlayerIndex();
    }

    public void RemoveFromList(Player other)
    {
        int index = _listings.FindIndex(x => x.Player == other);
        if (index != -1)
        {
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
        UpdatePlayerIndex();
    }

    public void DestroyList()
    {
        foreach (PlayerListing listing in _listings)
        {
            Destroy(listing.gameObject);
        }
        _listings.Clear();
    }

    public bool CheckAllReady()
    {
        bool allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object IsReady;
            if(player.CustomProperties.TryGetValue("Ready", out IsReady))
            {
                if((bool)IsReady == false)
                    allReady = false;
            }
        }

        return allReady;
    }

    void UpdatePlayerIndex()
    {
        int index = 1;
        foreach (PlayerListing listing in _listings)
        {
            listing.SetIndex(index++);
        }
    }
}
