using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BabuBehaviour : MonoBehaviour
{
    private enum States
    {
        Spawn, //spawn in
        FindCover, //ignore DONE
        Shoot, //
        Firing, //
        Strafe, // ignore? no, I think I'll make it so that if they're close, they effectively use strafe
        Die, //Initialise death subroutine and then enter Empty
        Empty //Do nothing as death resolves
    }

    [SerializeField] private States state = States.Spawn;
    [SerializeField] private NavMeshAgent nav;
    [SerializeField] private GameObject player;
    [SerializeField] private Gun_Controller gun;
    [SerializeField] private float minStrafeRange, maxStrafeRange;
    [SerializeField] private float hp = 2;
    [SerializeField] private Animator anim;
    [SerializeField] private LaserMail mail;
    private Quaternion playerLook;
    private float initacc;

    // Start is called before the first frame update
    void Start()
    {
        mail = GetComponent<LaserMail>();
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
        initacc = nav.speed;
        gun = GetComponentInChildren<Gun_Controller>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
        if (player == null)
        {
            //Debug.Log("Enemy can't find player");
            Destroy(transform.parent.gameObject);
        }
        NavMeshHit navPlacer;
        if(NavMesh.SamplePosition(transform.position, out navPlacer, 500, 1))
        {
            gameObject.transform.position = navPlacer.position;
            nav.enabled = true;
        }
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hp -= mail.damageSince;
        mail.damageSince = 0;
        if (hp <= 0 && state!=States.Empty)
        {
            state = States.Die;
        }
        playerLook = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);

        switch (state)//ment
        {
            case States.Spawn:
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "SpiderSpawn")
                    state = getNewState();
                break;
            case States.FindCover://ignore (if we end up here, go elsewhere)
                state = getNewState();
                break;

            case States.Shoot:
                //check line of sight
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                RaycastHit shootChecker = new RaycastHit();
                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider != null && shootChecker.collider.gameObject == player || Vector3.Distance(transform.position, player.transform.position) < 0.1) //did we hit the player when targeted?
                {
                    //fire
                    if (callOfDutyShootAMan())
                    {
                        state = States.Firing;
                        anim.SetBool("Attack", true);
                    }
                    else
                    {
                        nav.SetDestination(transform.position);
                    }
                }
                else
                {
                    //move towards player
                    nav.SetDestination(player.transform.position);
                }
                break;

            case States.Firing:
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                nav.speed = 0;
                nav.SetDestination(player.transform.position);
                if (gun.State < 2)
                {
                    StartCoroutine(Animdelay());
                    nav.speed = initacc;
                    nav.SetDestination(transform.position);
                    state = getNewState();
                }
                break;

            case States.Strafe:
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                if (Vector3.Distance(player.transform.position, nav.destination) > maxStrafeRange || Vector3.Distance(player.transform.position, nav.destination) < minStrafeRange)
                {
                    StrafeTarget(0);
                }

                if (Vector3.Distance(nav.destination, transform.position) < 0.5 || gun.State == 0)//if close enough to where was going
                {
                    state = getNewState();//reroll priorities
                }
                break;

            case States.Die:
                //GetComponentInChildren<Animator>().enabled = false;//deactivate the animator
                anim.SetTrigger("Death");

                GetComponent<CapsuleCollider>().enabled = false;
                if (Random.value <= 0.1)
                    gun.Fire(9, true);

                transform.parent.DetachChildren();
                nav.SetDestination(transform.position);
                state = States.Empty; //do no more after the next line
                Destroy(gameObject, Random.Range(0.5f, 1)); //remove this, the prefab model, but leave the bullets
                break;
            case States.Empty:
                break;
            default:
                break;
        }

    }

    IEnumerator Animdelay()
    {
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Attack", false);
    }

    private void StrafeTarget(int attempt)
    {
        if (attempt > 10)
        {
            getNewState();
            return;
        }

        Vector3[] targets = new Vector3[4];
        float[] dists = new float[4];

        float offset = Random.Range(0, maxStrafeRange - minStrafeRange);
        offset += minStrafeRange;
        Vector3 randomPoint = Random.insideUnitCircle;
        randomPoint.z = randomPoint.y;
        randomPoint.y = 0;
        randomPoint.Normalize();
        randomPoint *= offset;

        targets[0] = randomPoint + player.transform.position;
        targets[1] = -randomPoint + player.transform.position;
        targets[2] = new Vector3(randomPoint.z, 0, -randomPoint.x) + player.transform.position;
        targets[3] = -targets[2] + player.transform.position;

        for (int i = 0; i < 4; i++)
        {
            dists[i] = Vector3.Distance(transform.position, targets[i]);
        }
        float mindist = Mathf.Min(dists);
        for (int i = 0; i < 4; i++)
        {
            if (dists[i] == mindist)
            {
                NavMeshPath path = new NavMeshPath();
                nav.SetDestination(targets[i]);
                nav.CalculatePath(targets[i], path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    return;
                }
            }
        }
        StrafeTarget(attempt + 1);
    }

    private bool callOfDutyShootAMan()
    {
        if(gun.State == 0)
        {
            gun.Fire();
            return true;
        }
        return false;
    }

    private States getNewState()
    {
        if (gun.State == 0)
            return States.Shoot;
        StrafeTarget(0);
            return States.Strafe;
    }
    private void OnCollisionEnter(Collision other)
    {
        if ((LayerMask.NameToLayer("BulletKiller") == other.gameObject.layer || LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer) && state != States.Empty && state != States.Die)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Player Bullets"))
            {
                hp-=other.gameObject.GetComponent<PlayerBullet>().damage;
            }
            if (hp <= 0)
            {
                state = States.Die;
            }
            Destroy(other.gameObject);
        }
    }
}
