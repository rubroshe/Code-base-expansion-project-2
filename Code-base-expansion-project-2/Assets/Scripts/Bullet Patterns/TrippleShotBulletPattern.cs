using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrippleShotBulletPattern : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("This Bullet Pattern Script Fires sequence, with the center aimed at the player")]
    public float Spread; //time between shots
    public float BulletSpeeds;
    public int BulletCount;
    public GameObject BulletType;
    private GameObject player;
    [Header("This entity spawned these bullets (assigned by this script)")]
    public Gun_Controller Spawner;
    private bool running = false;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
            Spawner = transform.parent.parent.GetComponentInChildren<Gun_Controller>();
        GameObject[] Group = FindObjectsOfType<GameObject>();
        player = null;
        foreach (GameObject g in Group)
        {
            if (g.CompareTag("Player"))
                player = g;
        }
        if (player == null)
        {
            //Debug.Log("Bullet can't find player");
            Destroy(this.gameObject);
        }
        Vector3 Direction = player.transform.position - transform.position;
        Direction.y = 0;
        Direction.Normalize();
        transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
        StartCoroutine(Shooter());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0 && !running)
        {
            Destroy(gameObject);
        }
        if (Spawner == null)
        {
            StopAllCoroutines();
            running = false;
        }
    }

    IEnumerator Shooter()
    {
        running = true;
        Vector3 tempDir = player.transform.position - transform.position;
        tempDir.y = 0;
        GameObject newbullet = Instantiate(BulletType, transform.position, Quaternion.identity, transform);
        newbullet.transform.rotation = Quaternion.LookRotation(tempDir, Vector3.up);
        newbullet.GetComponent<DefaultBullet>().Speed = BulletSpeeds;
        newbullet.GetComponent<DefaultBullet>().Direction = new Vector3(0, 0, 1);
        float internalTime = 0;
        for (int i = 1; i < BulletCount; i++)
        {
            while(internalTime < Spread)
            {
                internalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            internalTime -= Spread;
            newbullet = Instantiate(BulletType, transform.position, Quaternion.identity, transform);
            newbullet.transform.rotation = Quaternion.LookRotation(tempDir, Vector3.up);
            newbullet.GetComponent<DefaultBullet>().Speed = BulletSpeeds;
            newbullet.GetComponent<DefaultBullet>().Direction = new Vector3(0, 0, 1);
        }


        if (Spawner != null)
            Spawner.CompleteFire();
        running = false;
    }
}
