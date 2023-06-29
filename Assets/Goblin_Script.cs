using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Goblin_Script : Utility_Functions, BasicItemStats_Interface, EnemyMovementStats_Interface, CommonVariables_Interface
{
    public int Health { get; set; } = 8;
    public float MovementSpeed { get; set; } = 1.1f;
    public float PercentageOfSpeedLostWhenHit { get; set; } = 0.5f;
    public int AmountOfTimeStoppedAfterHit { get; set; } = 1;

    public new bool DamagesPlayerOnContact { get; set; } = true;



    //Components
    FollowPath_Script GoblinFollowPath_Script;
    // End Components

    void Start()
    {
        GoblinFollowPath_Script = GetComponent<FollowPath_Script>();
        GoblinFollowPath_Script.Aim = FollowPath_Script.PathingAim.ChaseKnight;
    }

    // void Start()
    // {
    //     //Components
    //     KnightScriptComponenet = GameObject.FindGameObjectWithTag("Player").GetComponent<Knight_Script>();
    //     GoblinRigidBodyComponent = GetComponent<Rigidbody2D>();
    //     GoblinFollowPath_Script = GetComponent<FollowPath_Script>();
    //     GoblinSeekerComponent = GetComponent<Seeker>();
    //     // End Components

    //     InvokeRepeating("UpdatePath", 0f, 0.3f);

    // }
    // void UpdatePath()
    // {
    //     if (!seekerBusy)
    //     {
    //         GoblinSeekerComponent.StartPath(GoblinRigidBodyComponent.position, KnightScriptComponenet.KnightRigidBodyComponent.position, OnPathBuilt);
    //     }
    // }

    // void OnPathBuilt(Path TempPath)
    // {
    //     if (!TempPath.error)
    //     {
    //         GoblinFollowPath_Script.AddNewPathToFollow(TempPath);
    //     }
    // }


}
