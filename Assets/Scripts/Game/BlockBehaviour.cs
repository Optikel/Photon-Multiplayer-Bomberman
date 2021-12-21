using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class BlockBehaviour : MonoBehaviourPun
{
    [System.Serializable]
    public class PowerUps
    {
        public GameObject Prefab;
        public float WeightedChance;
    }
    
    [SerializeField]
    List<PowerUps> PowerUpList;

    float TotalWeight = 0;
    GameObject PowerUpContainer;
    // Start is called before the first frame update
    void Start()
    {
        PowerUpContainer = GameObject.Find("PowerUpContainer");
        if (PowerUpContainer == null)
            Debug.LogError("PowerUpContainer Missing in BlockBehaviour!");

        ResetWeight();
    }

    public void SpawnPowerUp()
    {
        if (photonView.IsMine)
        {
            ResetWeight();
            SpawnWeightedRandomPowerUp();
        }
    }

    private void SpawnWeightedRandomPowerUp()
    {
        float Value = Random.value;

        foreach(PowerUps powerup in PowerUpList)
        {
            if(Value < powerup.WeightedChance)
            {
                if(powerup.Prefab != null)
                {
                    PhotonNetwork.Instantiate(powerup.Prefab.name, transform.position, powerup.Prefab.transform.rotation);
                }
                return;
            }

            Value -= powerup.WeightedChance;
        }

        Debug.LogError("Invalid Config! Could not spawn weighted random powerup. Did you forget to call ResetWeight?");
    }

    private void ResetWeight()
    {
        TotalWeight = 0;
        foreach (PowerUps powerup in PowerUpList)
        {
            TotalWeight += powerup.WeightedChance;
        }

        foreach (PowerUps powerup in PowerUpList)
        {
            powerup.WeightedChance = powerup.WeightedChance / TotalWeight;
        }
    }
    
}
