using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject currentWeapon;
    public int health = 100;

    public Vector3 initLerp;
    public Vector3 finalLerp;
    private Vector3 initPosition;

    public Text Message;
    public Text gameMessage;
    public Text healthStatus;

    private bool isLerping = false;
    private float timeForLerpA = 0.25f;
    private float elaspedTimeA = 0f;
    private float timeForLerpB = 0.25f;
    private float elaspedTimeB = 0f;
    private int goblinDeaths = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Message = GameObject.Find("Message").GetComponent<Text>();
        gameMessage = GameObject.Find("CombatMessage").GetComponent<Text>();
        healthStatus = GameObject.Find("Health").GetComponent<Text>();
        initPosition = currentWeapon.transform.position;
        initLerp = currentWeapon.transform.rotation.eulerAngles;
        finalLerp = currentWeapon.transform.rotation.eulerAngles + new Vector3(0f, 0f, 90f);
        Debug.Log(initLerp);
        Debug.Log(finalLerp);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit HitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f))
        {
            Message.text = HitInfo.transform.name;
        }

        if (isLerping == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isLerping = true;
                elaspedTimeA = 0f;
                elaspedTimeB = 0f;
            }
        }
        else
        {
            if (elaspedTimeA < timeForLerpA)
            {
                currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.Lerp(initLerp, finalLerp, elaspedTimeA / timeForLerpA));
                elaspedTimeA += Time.deltaTime;
            }
            else if (elaspedTimeB < timeForLerpB)
            {
                currentWeapon.transform.localRotation = Quaternion.Euler(Vector3.Lerp(finalLerp, initLerp, elaspedTimeB / timeForLerpB));
                elaspedTimeB += Time.deltaTime;
            }
            else
            {
                isLerping = false;
                Debug.Log(initLerp);
                Debug.Log(finalLerp);

                RaycastHit HitInfoAttack;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfoAttack, 5.0f))
                {
                    if (HitInfoAttack.transform.gameObject.name.StartsWith("GOBLIN"))
                    {
                        GoblinAI goblin = HitInfoAttack.transform.GetComponent<GoblinAI>();
                        int atkRNG = UnityEngine.Random.Range(0, 10);
                        if (atkRNG > 5)
                        {
                            goblin.health -= 20;
                            GameLog("You hit Goblin for 20 hp");
                            GameLog(goblin.health.ToString());
                            if (goblin.health <= 0)
                            {
                                GameLog("Goblin is dead");
                                goblinDeaths += 1;
                                GameLog(goblinDeaths.ToString() + " Goblin deaths");
                                Destroy(HitInfoAttack.transform.gameObject);
                            }
                        }
                        else
                        {
                            GameLog("You failed to hit Goblin");
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.name);

        if (other.transform.name == "CharacterDetect") {
            GoblinAI goblin = other.transform.parent.gameObject.GetComponent<GoblinAI>();
            goblin.Attack();
            goblin.steeringBasics.maxVelocity = 0.0f;
        }
        else if (other.transform.name == "PotentialPickup")
        {
            //maybe increase gold here?

        }

    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.transform.name);

        if (other.transform.name == "CharacterDetect")
        {
            GoblinAI goblin = other.transform.parent.gameObject.GetComponent<GoblinAI>();
            if (goblin.anims.isPlaying == false)
            {
                int atkRNG = UnityEngine.Random.Range(0, 10);
                if (atkRNG > 5)
                {
                    this.health -= 20;
                    GameLog("Goblin hit you for 20 hp");
                    HealthLog((this.health).ToString());


                }
                else
                {
                    GameLog("Goblin failed to hit");
                }
            }
            goblin.Attack();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.transform.name);

        if (other.transform.name == "CharacterDetect")
        {
            GoblinAI goblin = other.transform.parent.gameObject.GetComponent<GoblinAI>();
            goblin.Run();
            goblin.steeringBasics.maxVelocity = 3.5f;
        }

    }

    int lnTotal = 0;
    private void GameLog(string ln)
    {
        if (lnTotal > 10)
        {
            gameMessage.text = "";
            lnTotal = 0;
        }

        gameMessage.text += ln + Environment.NewLine;
        ++lnTotal;
    }

    private void HealthLog(string ln)
    {
        healthStatus.text = ln + "%";
    }
}

