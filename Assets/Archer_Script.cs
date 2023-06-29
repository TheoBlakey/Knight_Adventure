using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Archer_Script : Utility_Functions, BasicItemStats_Interface, EnemyMovementStats_Interface, CommonVariables_Interface
{
    public int Health { get; set; } = 8;
    public float MovementSpeed { get; set; } = 1.1f;
    public float PercentageOfSpeedLostWhenHit { get; set; } = 0.1f;
    public int AmountOfTimeStoppedAfterHit { get; set; } = 0;

    //Components
    Knight_Script KnightScriptComponenet;
    Rigidbody2D ArcherRigidBodyComponent;
    FollowPath_Script ArcherFollowPath_Script;
    Seeker ArcherSeekerComponent;
    // End Components

    public GameObject ArrowRef;


    // public bool RecentlySeenKnight = false;
    // bool BeingChased = false;
    // float previousDistanceForKnight = 0;
    public bool ShotRecently = false;
    // public bool StickingToOneRunAwayPath = false;


    bool AttemptingToRunAway = false;
    float DistanceWhenTryingToRunAway = 0;


    Vector2 KnightLocationLastUpdate = new Vector2(0, 0);
    Vector2 ArcherLocationLastUpdate = new Vector2(0, 0);

    public bool RunAwayReason = false;



    void Start()
    {
        //Components
        KnightScriptComponenet = GameObject.FindGameObjectWithTag("Player").GetComponent<Knight_Script>();
        ArcherRigidBodyComponent = GetComponent<Rigidbody2D>();
        ArcherFollowPath_Script = GetComponent<FollowPath_Script>();
        ArcherSeekerComponent = GetComponent<Seeker>();
        // End Components


        ArcherFollowPath_Script.RunawayRandomAmount = 1;
    }

    void Update()
    {

        bool CanSeeKnight = !(Physics2D.Linecast(ArcherRigidBodyComponent.position, GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>().position, 64));

        if (CanSeeKnight && !ShotRecently)
        {
            ActivateTimerOnBool(0.5f, "ShotRecently");
            Quaternion rotation = new Quaternion();
            GameObject ArrowObj = Instantiate(ArrowRef, ArcherRigidBodyComponent.position, rotation);
            Arrow_Script ArrowScript = ArrowObj.GetComponent<Arrow_Script>();
            ArrowScript.IsEvil = true;

        }


        bool RunningAwayAndKnightStillCloser = false;
        float currentKnightDistance = Vector2.Distance(ArcherRigidBodyComponent.position, KnightScriptComponenet.KnightRigidBodyComponent.position);

        if (AttemptingToRunAway)
        {
            if (currentKnightDistance < DistanceWhenTryingToRunAway)
            {
                RunningAwayAndKnightStillCloser = true;
            }
            DistanceWhenTryingToRunAway = currentKnightDistance;
        }


        bool KnightMovingToGetCloser = false;
        float lastDistance = Vector2.Distance(ArcherLocationLastUpdate, KnightLocationLastUpdate);
        float CurrentDistance = Vector2.Distance(ArcherLocationLastUpdate, KnightScriptComponenet.KnightRigidBodyComponent.position);

        if (CurrentDistance < lastDistance)
        {
            KnightMovingToGetCloser = true;
        }

        KnightLocationLastUpdate = KnightScriptComponenet.KnightRigidBodyComponent.position;
        ArcherLocationLastUpdate = ArcherRigidBodyComponent.position;


        if (CanSeeKnight || RunningAwayAndKnightStillCloser || KnightMovingToGetCloser)
        {
            ActivateTimerOnBool(0.4f, "RunAwayReason");
        }



        if (RunAwayReason)
        {
            AttemptingToRunAway = true;
            ArcherFollowPath_Script.Aim = FollowPath_Script.PathingAim.RunawayFromKnight;

            DistanceWhenTryingToRunAway = currentKnightDistance;

        }
        else
        {
            AttemptingToRunAway = false;
            ArcherFollowPath_Script.Aim = FollowPath_Script.PathingAim.ChaseKnight;

        }

    }

    // bool FourCornersRayCastCheck(Vector2 locationFromCheck, Vector2 objectToBeSeen)
    // {
    //     float d = 0.5f; //distanceFromCenterToCorner

    //     if (CheckIfObjectCanBeSeen(locationFromCheck, objectToBeSeen))
    //     { return true; }

    //     if (CheckIfObjectCanBeSeen(new Vector2(locationFromCheck.x + d, locationFromCheck.y), objectToBeSeen))
    //     { return true; }

    //     if (CheckIfObjectCanBeSeen(new Vector2(locationFromCheck.x + d, locationFromCheck.y + d), objectToBeSeen))
    //     { return true; }

    //     if (CheckIfObjectCanBeSeen(new Vector2(locationFromCheck.x - d, locationFromCheck.y), objectToBeSeen))
    //     { return true; }

    //     if (CheckIfObjectCanBeSeen(new Vector2(locationFromCheck.x - d, locationFromCheck.y - d), objectToBeSeen))
    //     { return true; }

    //     return false;
    // }

    // bool CheckIfObjectCanBeSeen(Vector2 locationFromCheck, Vector2 objectToBeSeen)
    // {
    //     List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    //     ContactFilter2D contactFilter2D = new ContactFilter2D();
    //     contactFilter2D.SetLayerMask(64); //walls, colliders and player
    //     contactFilter2D.useTriggers = true;

    //     Vector2 direction = (objectToBeSeen - locationFromCheck).normalized;
    //     float distance = Vector2.Distance(locationFromCheck, objectToBeSeen);

    //     int castCount = ArcherRigidBodyComponent.Cast(
    //             direction,
    //             contactFilter2D,
    //             castCollisions,
    //             distance
    //             );

    //     if (castCount == 0)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }
}
