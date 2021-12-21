using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PowerUpScript : MonoBehaviourPun
{
    public enum PowerUpType
    {
        BombUp,
        FireUp,
        SpeedUp,
        Kick
    }

    public PowerUpType Type;
    // Start is called before the first frame update
    void Start()
    {
        GameObject container = GameObject.Find("PowerUpContainer");
        if (container != null)
            transform.parent = GameObject.Find("PowerUpContainer").transform;
        else
            Debug.LogError("PowerUpContainer Missing!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
            return; 

        PlayerInstantiation player;
        if (other.TryGetComponent<PlayerInstantiation>(out player))
        { 
            PhotonNetwork.Destroy(photonView);
            switch (Type)
            {
                case PowerUpType.BombUp:
                    player.photonView.RPC("SetBombCount", RpcTarget.AllBuffered, player.BombCount + 1);
                    break;
                case PowerUpType.FireUp:
                    player.photonView.RPC("SetFirePower", RpcTarget.AllBuffered, player.PowerIncrease + 1);
                    break;
                case PowerUpType.SpeedUp:
                    player.photonView.RPC("SetSpeed", RpcTarget.AllBuffered, player.SpeedMultiplier + 1);
                    break;
                case PowerUpType.Kick:
                    break;
            }
        }

    }
}
