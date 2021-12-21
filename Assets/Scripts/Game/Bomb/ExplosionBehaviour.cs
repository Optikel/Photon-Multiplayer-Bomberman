using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ExplosionBehaviour : MonoBehaviourPun
{
    [SerializeField]
    Material NonPenetrativeMaterial;
    [SerializeField]
    Material PenetrativeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        BombBehaviour parentbomb;

        if (transform.parent.TryGetComponent<BombBehaviour>(out parentbomb))
        {
            GetComponent<MeshRenderer>().material = parentbomb.Penetrative ? PenetrativeMaterial : NonPenetrativeMaterial;
        }
        else
        {
            Debug.LogError("BombBehaviour is not parent of Explosion");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Destructable"))
        {
            Object.Destroy(other.gameObject);
            BlockBehaviour block;
            if (other.TryGetComponent<BlockBehaviour>(out block))
            {
                block.SpawnPowerUp();
            }
        }

        if (other.CompareTag("Player"))
        {
            //Destroy Player on masterclient side - avoid error
            if (other.GetComponent<PhotonView>().IsMine)
            {
                PhotonNetwork.Destroy(other.GetComponent<PhotonView>());

                Hashtable props = other.GetComponent<PhotonView>().Owner.CustomProperties;
                if (props.ContainsKey(GameManager.PLAYER_ALIVE))
                {
                    props[GameManager.PLAYER_ALIVE] = false;
                }
                other.GetComponent<PhotonView>().Owner.SetCustomProperties(props);
            }

            //Instantiate SpectatorView
            if(photonView.IsMine)
                PhotonNetwork.Instantiate("SpectatorView", Vector3.zero, Quaternion.identity);
        }

        if (other.CompareTag("Bombs"))
        {
            BombBehaviour bomb;
            if (other.TryGetComponent<BombBehaviour>(out bomb))
            {
                bomb.ChainExplode();
            }
        }
    }

    [PunRPC]
    void AttachToBomb(int viewID)
    {
        transform.parent = PhotonView.Find(viewID).transform;
    }
}
