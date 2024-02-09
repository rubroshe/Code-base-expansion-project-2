using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawControl : MonoBehaviour
{

    public GameObject player;
    public BoxCollider chainsaw;
    public ParticleSystem chainsawSparks;

    public void RevSaw()
    {
        chainsaw.enabled = true;
        chainsawSparks.gameObject.SetActive(true);

    }

    public void StopSaw()
    {
        chainsawSparks.gameObject.SetActive(false);
        chainsaw.enabled = false;
        player.GetComponent<PlayerGun>().chainsawing = false;

    }

}
