using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderMommyBehaviour : MonoBehaviour
{
    private enum States
    {
        Spawn, //Done
        FindCover, //ignore DONE
        Shoot, //Done
        Firing, //Done
        Strafe, //done?
        Die, //done?
        Empty//done
    }

    [SerializeField] private bool distant = true; //true if distance to player > maxstraferange (typically)

    //[SerializeField] private GameObject SpecialLittleOrby;

    [SerializeField] private States state = States.Spawn;
    [SerializeField] private NavMeshAgent nav;
    [SerializeField] private GameObject player;
    [SerializeField] private Gun_Controller gun;
    [SerializeField] private float minStrafeRange, maxStrafeRange;
    [SerializeField] private float hp = 7;
    [Header("Attack Odd value between 0 and 1, chance to attack")]
    [SerializeField] private float attackOdds = 0.5f;
    [SerializeField] private Animator anim;
    [SerializeField] private LaserMail mail;
    private Quaternion playerLook;
    public float playerDist;
    private float initacc;
    [SerializeField] private GameObject mySon;

    // Start is called before the first frame update
    void Start()
    {
        mail = GetComponent<LaserMail>();
        nav = GetComponent<NavMeshAgent>();
        initacc = nav.speed;
        gun = GetComponentInChildren<Gun_Controller>();
        player = FindObjectOfType<PlayerMovement>().gameObject;
        if (player == null)
        {
            //Debug.Log("Enemy can't find player");
            Destroy(transform.parent.gameObject);
        }
        NavMeshHit navPlacer;
        if (NavMesh.SamplePosition(transform.position, out navPlacer, 500, 1))
        {
            gameObject.transform.position = navPlacer.position;
            nav.enabled = true;
        }
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //SpecialLittleOrby.transform.position = nav.destination;
        hp -= mail.damageSince;
        mail.damageSince = 0;
        if (hp <= 0 && state != States.Empty)
        {
            state = States.Die;
        }
        playerLook = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        playerDist = Vector3.Distance(player.transform.position, transform.position);
        switch (state)//ment
        {
            case States.Spawn:
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "SpiderSpawn")
                {
                    StrafeTarget(0);
                    state = States.Strafe;
                }

                break;
            case States.FindCover:
                state = getNewState();
                break;

            case States.Shoot:
                //check line of sight
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                RaycastHit shootChecker = new RaycastHit();
                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider!=null && shootChecker.collider.gameObject == player) //did we hit the player when targeted?
                {
                    //fire
                    if (callOfDutyShootAMan())
                    {
                        state = States.Firing;
                        //animation bool on goes in callOfDutyShootAMan()
                    }
                    else
                    {
                        nav.SetDestination(transform.position);
                    }
                }
                else
                {
                    //move towards player
                    //nav.SetDestination(player.transform.position);
                    state = getNewState();
                }
                break;

            case States.Firing:
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                nav.speed = 0;
                nav.SetDestination(player.transform.position);
                if (gun.State < 2)
                {
                    //animation bool turn off here
                    nav.speed = initacc;
                    nav.SetDestination(transform.position);
                    state = getNewState();
                }
                break;

            case States.Strafe:
                transform.rotation = Quaternion.Slerp(transform.rotation, playerLook, 0.3f);
                //Debug.DrawLine(nav.destination, transform.position, Color.blue, 7f);

                /*
                if (Vector3.Distance(player.transform.position, nav.destination) > maxStrafeRange || Vector3.Distance(player.transform.position, nav.destination) < minStrafeRange)
                {
                    StrafeTarget(0);
                }
                */
                /*
                Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
                if (shootChecker.collider.gameObject == player) //Can we shoot the player?
                {
                    state = getNewState();
                }
                */

                if (Vector3.Distance(nav.destination, transform.position) < 0.5)//if close enough to where was going
                {
                    state = getNewState();//reroll priorities
                }
                break;

            case States.Die:
                StartCoroutine(Death());
                anim.SetTrigger("Death");
                state = States.Empty;
                break;

            default:
                break;
        }
    }

    private bool callOfDutyShootAMan()
    {
        if (gun.State == 0)
        {
            float rand = Random.value;
            if (distant)
            {
                if (rand <= .7f)
                {
                    gun.BulletIndex = 3;
                }
                else if (rand <= .9f)
                {
                    gun.BulletIndex = 4;
                }
                else
                gun.BulletIndex = 5;
            }
            else if (rand <= .4f)
            {
                gun.BulletIndex = 4;
            }
            else
            gun.BulletIndex = 5;


            gun.FireOnDelay(0.5f);
            gun.State = 3;
            return true;
        }
        return false;
    }

    private States getNewState()
    {
        RaycastHit shootChecker = new RaycastHit();
        Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
        if (shootChecker.collider != null && shootChecker.collider.gameObject == player) //Can we shoot the player?
        {
            if (Random.value < attackOdds)
            {
                return States.Shoot;
            }
            distant = checkDistant();
            StrafeTarget(0);
            return States.Strafe;
        }
        distant = checkDistant();
        StrafeTarget(0);
        return States.Strafe;
    }

    private bool checkDistant()
    {
        return Random.value <= 0.5f;
        /*
        if (distant)
        {
            if (playerDist < minStrafeRange)
                return false;
            else if (Random.value <= 0.25)
            {
                return true;
            }
        }
        else
        {
            if (playerDist > maxStrafeRange)
                return true;
            else if (Random.value <= 0.25)
            {
                return false;
            }
        }
        return distant;*/
    }

    private void StrafeTarget(int attempt)
    {
        float maxRange, minRange;
        if (distant)
        {
            minRange = maxStrafeRange;
            maxRange = maxStrafeRange + minStrafeRange;
        }
        else
        {
            maxRange = minStrafeRange;
            minRange = minStrafeRange - 1;
        }
        if (attempt > 10)
        {
            getNewState();
            return;
        }

        Vector3[] targets = new Vector3[4];
        float[] dists = new float[4];

        float offset = Random.Range(0, maxRange - minRange);
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

    private void CoverTarget()//unused
    {
        RaycastHit shootChecker = new RaycastHit();
        Physics.Raycast(transform.position, player.transform.position - transform.position, out shootChecker, (player.transform.position - transform.position).magnitude);
        if (shootChecker.collider.gameObject == player) //did we hit the player when targeted?
        {
            //we are not currently in cover
            Vector3[] testpos = new Vector3[20];
            Vector3 playerToMe = transform.position - player.transform.position;
            playerToMe.y = 0f;
            playerToMe.Normalize();
            Vector3 testDir = playerToMe;
            testDir.x = -playerToMe.z;
            testDir.z = playerToMe.x;

            for (int i = 0; i < 10; i++)
            {
                testpos[2 * i] = transform.position + (i + 1) * testDir;
                testpos[2 * i + 1] = transform.position - (i + 1) * testDir;
            }

            //evens are one direction, odds are the other

            //for each testpos, check the raycast for cover
            for (int i = 0; i < 20; i++)
            {
                bool failure = false;
                RaycastHit wallCheck = new RaycastHit();
                Physics.Raycast(testpos[i], transform.position - testpos[i], out wallCheck);
                Physics.Raycast(testpos[i], player.transform.position - testpos[i], out shootChecker, (player.transform.position - testpos[i]).magnitude);
                if (shootChecker.collider.gameObject != player && wallCheck.collider != null && LayerMask.NameToLayer("Walls") != wallCheck.collider.gameObject.layer)
                {
                    //cover found
                    Vector3 coverTarg = shootChecker.point;
                    playerToMe = coverTarg - player.transform.position;
                    playerToMe.Normalize();
                    coverTarg += playerToMe;
                    Physics.Raycast(coverTarg, transform.position - coverTarg, out wallCheck);
                    if (wallCheck.collider != null && LayerMask.NameToLayer("Walls") != wallCheck.collider.gameObject.layer)
                    {
                        Collider[] FinalChecker = Physics.OverlapSphere(coverTarg, 0.1f);
                        foreach (Collider c in FinalChecker)
                        {
                            if (LayerMask.NameToLayer("Walls") == c.gameObject.layer)
                            {
                                failure = true;
                            }
                        }
                        if (!failure)
                        {
                            nav.SetDestination(coverTarg);
                            return;
                        }
                    }
                }
            }
            getNewState();
        }
    }

    private float Power(int e, int x)//unused
    {
        if (x == 0)
            return 1;
        int power = Mathf.Abs(x);
        float result = 1;
        for (int i = 0; i < power; i++)
        {
            result *= e;
        }
        if (x < 0)
            result = 1 / result;
        return result;
    }

    private void OnCollisionEnter(Collision other)
    {
        if ((LayerMask.NameToLayer("BulletKiller") == other.gameObject.layer ||  LayerMask.NameToLayer("Player Bullets") == other.gameObject.layer) && state != States.Empty && state != States.Die)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player Bullets"))
            {
                hp -= other.gameObject.GetComponent<PlayerBullet>().damage;
            }
            if (hp <= 0)
            {
                state = States.Die;
            }
            Destroy(other.gameObject);
        }
    }

    private IEnumerator Death()
    {
        nav.SetDestination(transform.position);
        GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Instantiate(mySon, transform.position, Quaternion.identity, transform.parent);
        Instantiate(mySon, transform.position, Quaternion.identity, transform.parent);
        float randmoval = Random.value;
        if (randmoval <= 0.1)
            gun.Fire(8, true);
        else if (randmoval <= 0.3)
            gun.Fire(9, true);
        else if (randmoval <= 0.4)
            gun.Fire(10, true);
        transform.parent.DetachChildren();
        Destroy(gameObject, Random.Range(0.5f, 1)); //remove this, the prefab model, but leave the bullets
    }
}
