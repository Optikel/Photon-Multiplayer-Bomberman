using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private Text _Index;
    [SerializeField]
    private Text _playerName;
    [SerializeField]
    private bool _ready;

    public Player Player { get; private set; }
    public void SetPlayerInfo(Player player)
    {
        Player = player;
        _playerName.text = player.NickName;
    }
}
