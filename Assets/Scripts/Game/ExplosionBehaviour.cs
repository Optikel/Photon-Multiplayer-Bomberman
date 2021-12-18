using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ExplosionBehaviour : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        }

        if (other.CompareTag("Player"))
        {
            //Destroy Player on masterclient side - avoid error
            if (other.GetComponent<PhotonView>().IsMine)
               PhotonNetwork.Destroy(other.GetComponent<PhotonView>());

            //Instantiate SpectatorView
            if(photonView.IsMine)
                PhotonNetwork.Instantiate("SpectatorView", Vector3.zero, Quaternion.identity);
        }

        if (other.CompareTag("Bombs"))
        {
        }
    }

    [PunRPC]
    void AttachToBomb(int viewID)
    {
        transform.parent = PhotonView.Find(viewID).transform;
    }
}
