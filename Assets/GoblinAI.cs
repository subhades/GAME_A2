using System.Collections;
using UnityEngine;
using UnityMovementAI;

public class GoblinAI : MonoBehaviour
{
    public SteeringBasics steeringBasics;
    public ArriveUnit arriveBehavior;
    public Animation anims;

    public GameObject player;
    public Player playerInfo;

    public int health = 100;
    public int attack = 5;
    public Vector3 spawnPosition;

    void Start()
    {
        player = GameObject.Find("PlayerCapsule");
        playerInfo = GameObject.Find("PlayerCameraRoot").GetComponent<Player>();

        steeringBasics = GetComponent<SteeringBasics>();
        arriveBehavior = GetComponent<ArriveUnit>();
        anims = GetComponent<Animation>();
        anims["attack3"].wrapMode = WrapMode.Once;

        arriveBehavior.enabled = false;
        arriveBehavior.targetPosition = player.transform.position;
        Idle();

        spawnPosition = transform.position;
    }

    void Update()
    {
        if (health > 0 && Vector3.Distance(transform.position, player.transform.position) < 10f)
        {
            if (!arriveBehavior.enabled)
            {
                Run();
            }
            arriveBehavior.enabled = true;
            arriveBehavior.targetPosition = player.transform.position;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        {
            StartCoroutine(GameObject.Find("GoblinManager").GetComponent<GoblinManager>().RespawnGoblin(this));
        }
    }

    public void Attack()
    {
        anims.Play("attack3");
    }

    public void Idle()
    {
        anims.Play("idle");
    }

    public void Run()
    {
        anims.Play("run");
    }
}
