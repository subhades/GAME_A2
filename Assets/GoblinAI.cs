using UnityEditor;
using UnityEngine;
using UnityMovementAI;

public class GoblinAI : MonoBehaviour
{
    public SteeringBasics steeringBasics;
    public ArriveUnit arriveBehavior;
    public FleeUnit fleeBehavior;

    public Animation anims;

    public GameObject player;
    public Player playerInfo;

    public int health = 100;
    public int attach = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("PlayerCapsule");
        playerInfo = GameObject.Find("PlayerCameraRoot").GetComponent<Player>();
        steeringBasics = this.GetComponent<SteeringBasics>();
        arriveBehavior = this.GetComponent<ArriveUnit>();
        anims = this.GetComponent<Animation>();
        anims["attack3"].wrapMode = WrapMode.Once;
        
        arriveBehavior.enabled = false;
        arriveBehavior.targetPosition = player.transform.position;
        Idle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, player.transform.position) < 10f)
        {
            if (arriveBehavior.enabled == false)
            {
                Run();
            }
            arriveBehavior.enabled = true;
        }

        if (health > 1)
        {
            arriveBehavior.targetPosition = player.transform.position;
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
