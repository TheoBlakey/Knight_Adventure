using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Thief_Script : Utility_Functions, BasicItemStats_Interface, EnemyMovementStats_Interface, CommonVariables_Interface
{

    public int Health { get; set; } = 5;
    public float MovementSpeed { get; set; } = 1.2f;
    public float PercentageOfSpeedLostWhenHit { get; set; } = 0f;
    public int AmountOfTimeStoppedAfterHit { get; set; } = 1;

    //Components
    Knight_Script KnightScriptComponenet;
    Rigidbody2D ThiefRigidBodyComponent;
    FollowPath_Script ThiefFollowPath_Script;
    Seeker ThiefSeekerComponent;
    // End Components

    public GameObject UpgradeRef;

    public bool _hasStolenBow = false;
    public bool CantStealBowBack = false;
    public bool HasStolenBow
    {
        get { return this._hasStolenBow; }
        set
        {
            SetChildActive(value, "StolenBow");
            if (_hasStolenBow)
            {
                DamagesPlayerOnContact = false;

            }
            else
            {
                DamagesPlayerOnContact = true;
            }
            this._hasStolenBow = value;


        }
    }


    void Start()
    {
        //Components
        KnightScriptComponenet = GameObject.FindGameObjectWithTag("Player").GetComponent<Knight_Script>();
        ThiefRigidBodyComponent = GetComponent<Rigidbody2D>();
        ThiefFollowPath_Script = GetComponent<FollowPath_Script>();
        ThiefSeekerComponent = GetComponent<Seeker>();
        // End Components

        ThiefFollowPath_Script.shouldFaceAim = false;
        ThiefFollowPath_Script.RunawayRandomAmount = 1;
        ThiefFollowPath_Script.Aim = FollowPath_Script.PathingAim.RunawayFromKnight;

    }



    void Update()
    {
        if (KnightScriptComponenet.UpgradeBow == false || CantStealBowBack)
        {
            ThiefFollowPath_Script.Aim = FollowPath_Script.PathingAim.RunawayFromKnight;
            ThiefFollowPath_Script.shouldFaceAim = false;
        }
        else
        {
            ThiefFollowPath_Script.Aim = FollowPath_Script.PathingAim.ChaseKnight;
            ThiefFollowPath_Script.shouldFaceAim = true;
        }

        // GameObject BowUpgrade = Instantiate(UpgradeRef, this.transform);
        // BowUpgrade.name = "BowUpgrade";
    }

    private void OnTriggerEnter2D(Collider2D otherThing)
    {

        if (otherThing.tag == "Player")
        {
            if (KnightScriptComponenet.UpgradeBow == true && !HasStolenBow)
            {
                KnightScriptComponenet.UpgradeBow = false;
                HasStolenBow = true;

            }
            else if (KnightScriptComponenet.UpgradeBow == false && HasStolenBow)
            {

                GameObject emptyGO = new GameObject();
                Transform newTransform = new GameObject().transform;
                newTransform.position = ThiefRigidBodyComponent.position;
                GameObject BowUpgrade = Instantiate(UpgradeRef, newTransform);
                BowUpgrade.name = "BowUpgrade";



                HasStolenBow = false;
                Invoke("ChangeBack", 2f);
                CantStealBowBack = true;

            }
        }
    }
    void ChangeBack()
    {
        CantStealBowBack = false;
    }

    // void UpdatePath()
    // {
    //     if (!ThiefFollowPath_Script.SeekerIsBusy())
    //     {
    //         // Vector2 randomLocationRunaway = new Vector2(Random.Range(-2, 2), Random.Range(-1, 1));
    //         Vector2 randomLocationRunaway = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-1.5f, 1.5f));
    //         ThiefSeekerComponent.StartPath(ThiefRigidBodyComponent.position, randomLocationRunaway, OnPathBuilt);
    //     }

    // }

    // void OnPathBuilt(Path TempPath)
    // {
    //     if (!TempPath.error)
    //     {
    //         float currentKnightDistance = Vector2.Distance(ThiefRigidBodyComponent.position, KnightScriptComponenet.KnightRigidBodyComponent.position);
    //         float proposedKnightDistance = Vector2.Distance(TempPath.vectorPath[1], KnightScriptComponenet.KnightRigidBodyComponent.position);

    //         Debug.Log("KNIGHT DISTANCE" + currentKnightDistance);

    //         bool happyToMoveThatWay = false;
    //         if (proposedKnightDistance > currentKnightDistance)
    //         {
    //             happyToMoveThatWay = true;
    //         }

    //         float randomNumber = Random.Range(1f, 1f);

    //         if ((randomNumber + currentKnightDistance) > 2)
    //         {
    //             happyToMoveThatWay = true;
    //         }

    //         if (happyToMoveThatWay)
    //         {
    //             ThiefFollowPath_Script.AddNewPathToFollow(TempPath);
    //         }
    //         else if (tryNumber < 10)
    //         {
    //             // Vector2 randomLocationRunaway = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-1.5f, 1.5f));
    //             // GoblinSeekerComponent.StartPath(ThiefRigidBodyComponent.position, randomLocationRunaway, OnPathBuilt);
    //             UpdatePath();
    //             tryNumber++;
    //         }

    //         tryNumber = 0;


    //     }
    // }

}
