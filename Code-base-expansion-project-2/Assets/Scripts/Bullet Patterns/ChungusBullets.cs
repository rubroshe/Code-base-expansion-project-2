using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChungusBullets : MonoBehaviour
{
    [Header("This Bullet Pattern Script Fires sequence, with the center aimed at the player")]
    public float Spread; //time between shots
    public float BulletSpeeds;
    public int BarrageCount;
    public GameObject BulletType;
    private GameObject player;
    public float Barrage1spread;
    public int Barrage1count;
    public float Barrage2spread;
    public int Barrage2count;
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
            Debug.Log("Bullet can't find player");
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
        tempDir.Normalize();
        GameObject newbullet = Instantiate(BulletType, transform.position, Quaternion.identity, transform);
        newbullet.transform.rotation = Quaternion.LookRotation(tempDir, Vector3.up);
        newbullet.GetComponent<ShotgunBulleter>().BulletSpeeds = BulletSpeeds;
        newbullet.GetComponent<ShotgunBulleter>().Spread = Barrage1spread;
        newbullet.GetComponent<ShotgunBulleter>().BulletCount = Barrage1count;
        newbullet.GetComponent<ShotgunBulleter>().overrider = true;
        float internalTime = 0;
        for (int i = 1; i < BarrageCount; i++)
        {
            while (internalTime < Spread)
            {
                internalTime += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            internalTime -= Spread;
            newbullet = Instantiate(BulletType, transform.position, Quaternion.identity, transform);
            newbullet.transform.rotation = Quaternion.LookRotation(tempDir, Vector3.up);
            newbullet.GetComponent<ShotgunBulleter>().BulletSpeeds = BulletSpeeds;
            newbullet.GetComponent<ShotgunBulleter>().Spread = Barrage2spread;
            newbullet.GetComponent<ShotgunBulleter>().BulletCount = Barrage2count;
            newbullet.GetComponent<ShotgunBulleter>().overrider = true;
        }


        if (Spawner != null)
            Spawner.CompleteFire();
        running = false;
    }
}
