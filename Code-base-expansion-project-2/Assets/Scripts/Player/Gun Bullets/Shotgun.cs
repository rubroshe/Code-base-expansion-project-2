using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{

    public GameObject bullet;
    public float spread = 30f;
    public float speed = 7;
    public int bulletCount = 7;

    public void Fire(GameObject holder, Transform origin)
    {

        int side = Mathf.RoundToInt((bulletCount - 1) / 2);

        Quaternion baseRotation = origin.rotation * Quaternion.Euler(90, 0, 0);

        for (int i = 0; i < bulletCount; i++)
        {
            GameObject newBullet;
            if (i == 0)
            {
                newBullet = GameObject.Instantiate(bullet, origin.position, baseRotation, holder.transform);
            }
            else if(i > 0 && i < side)
            {
                newBullet = GameObject.Instantiate(bullet, origin.position, baseRotation, holder.transform);
                newBullet.transform.rotation *= Quaternion.Euler(0, 0, Random.Range(3, spread / 2));
            }
            else
            {
                newBullet = GameObject.Instantiate(bullet, origin.position, baseRotation, holder.transform);
                newBullet.transform.rotation *= Quaternion.Euler(0, 0, -Random.Range(3, spread / 2));
            }

            newBullet.GetComponent<Rigidbody>().velocity = newBullet.transform.up * speed;
        }
    }

}
