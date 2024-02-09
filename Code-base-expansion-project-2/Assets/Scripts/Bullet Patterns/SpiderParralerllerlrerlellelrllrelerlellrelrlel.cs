using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderParralerllerlrerlellelrllrelerlellrelrlel : MonoBehaviour
{
    public float interBulletaryDistance;
    public float speed;
    public float maxDist;
    public Gun_Controller Spawner;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
            Spawner = transform.parent.parent.GetComponentInChildren<Gun_Controller>();
        GameObject[] Group = FindObjectsOfType<GameObject>();
        GameObject player = null;
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
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity, transform);
        newBullet.transform.localPosition += new Vector3(interBulletaryDistance / 2, 0, 0);
        newBullet.GetComponent<DefaultBullet>().Speed = speed;
        newBullet.GetComponent<DefaultBullet>().Direction = new Vector3(0, 0, 1);
        newBullet.GetComponent<DefaultBullet>().MaxTravel = maxDist;
        newBullet = Instantiate(bullet, transform.position, Quaternion.identity, transform);
        newBullet.transform.localPosition -= new Vector3(interBulletaryDistance / 2, 0, 0);
        newBullet.GetComponent<DefaultBullet>().Speed = speed;
        newBullet.GetComponent<DefaultBullet>().Direction = new Vector3(0, 0, 1);
        newBullet.GetComponent<DefaultBullet>().MaxTravel = maxDist;

        if (Spawner != null)
            Spawner.CompleteFire();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
