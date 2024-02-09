using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        //DeleteMe();
        Invoke(nameof(DeleteMe), 1.5f);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
        {
            gameObject.GetComponent<Rigidbody>().useGravity = enabled;
            Invoke(nameof(DeleteMe), 3f);
            damage = 0;
        }
        //DeleteMe();
    }

    private void DeleteMe()
    {
        Destroy(gameObject);
    }
}
