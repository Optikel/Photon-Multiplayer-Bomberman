using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;
public class PlayerListing : MonoBehaviour
{
    [SerializeField]
    private Text _Index;
    [SerializeField]
    private Text _playerName;
    [SerializeField]
    private Text _readyText;

    public Player Player { get; private set; }
    public void SetPlayerInfo(Player player, int index)
    {
        Player = player;
        _Index.text = index.ToString();
        _playerName.text = player.NickName;

        Hashtable hash = new Hashtable();
        hash.Add("Ready", false);
        Player.SetCustomProperties(hash);
        _readyText.text = (bool)hash["Ready"] ? "Ready" : "Not Ready";
    }
    public void SetIndex(int index) { _Index.text = index.ToString(); }
    public void SetReadyText(string text) { _readyText.text = text; }
}
