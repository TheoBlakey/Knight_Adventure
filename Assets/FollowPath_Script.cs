using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FollowPath_Script : Utility_Functions
{
    EnemyMovementStats_Interface EnemyMovementStats_Interface;

    //Components
    Knight_Script KnightScriptComponenet;
    Rigidbody2D GenericRigidBodyComponent;
    SpriteRenderer GenericSpriteRendererComponent;
    Seeker GenericSeekerComponent;
    // End Components

    Vector2 randomLocation = new Vector2(0, 0);
    public bool shouldFaceAim = true;
    public bool CanMoveThroughEnemies = false;
    Path pathToFollow;
    int currentWayPointNumber = 0;
    bool lookingRight = true;
    public bool immediateStop = false;

    // Possible for still = stuck
    // int tryNumber = 0;
    // Vector2 PreviousLocation = new Vector2(100, 100);


    public bool RecentlyMovedSucessfully = false;
    public bool StuckSoRandomPathing = false;

    public PathingAim _aim = PathingAim.None;
    public PathingAim Aim
    {
        get { return this._aim; }
        set
        {
            if (_aim != value)
            {
                AimHasChanged = true;
            }
            this._aim = value;

        }
    }

    public float RunawayRandomAmount = 0;
    int SafetyTryNumber = 0;
    bool AimHasChanged = false;

    public enum PathingAim
    { None, ChaseKnight, RunawayFromKnight, ChaseAnotherObject }
    public GameObject ObjectToChase;

    public bool SeekerIsBusy()
    {
        if (StuckSoRandomPathing || Aim != PathingAim.None)
        { return true; }
        else
        { return false; }
    }

    public void AddNewPathToFollow(Path newPathToFollow)
    {
        if (!StuckSoRandomPathing)
        {
            pathToFollow = newPathToFollow;
            currentWayPointNumber = 0;
        }

    }

    void Start()
    {
        EnemyMovementStats_Interface = GetComponent<EnemyMovementStats_Interface>();

        //Components
        GenericRigidBodyComponent = GetComponent<Rigidbody2D>();
        KnightScriptComponenet = GameObject.FindGameObjectWithTag("Player").GetComponent<Knight_Script>();
        GenericSpriteRendererComponent = GetComponent<SpriteRenderer>();
        GenericSeekerComponent = GetComponent<Seeker>();
        // End Components

        InvokeRepeating("BuildAPath", 0f, 0.3f);

    }

    void BuildAPath()
    {
        if (StuckSoRandomPathing)
        {
            GenericSeekerComponent.StartPath(GenericRigidBodyComponent.position, randomLocation, OnPathBuilt);
        }
        else
        {
            switch (Aim)
            {
                case PathingAim.None:
                    {
                        AimHasChanged = false;
                        pathToFollow = null;
                        break;
                    }
                case PathingAim.ChaseKnight:
                    {
                        GenericSeekerComponent.StartPath(GenericRigidBodyComponent.position, KnightScriptComponenet.KnightRigidBodyComponent.position, OnPathBuilt);
                        AimHasChanged = false;
                        break;
                    }
                case PathingAim.RunawayFromKnight:
                    {
                        FindRunawayPath();
                        AimHasChanged = false;
                        break;
                    }
                case PathingAim.ChaseAnotherObject:
                    {
                        if (ObjectToChase != null)
                        {
                            GenericSeekerComponent.StartPath(GenericRigidBodyComponent.position, ObjectToChase.GetComponent<Rigidbody2D>().position, OnPathBuilt);
                            AimHasChanged = false;

                        }
                        break;
                    }
                default: break;
            }

        }

    }

    void FindRunawayPath()
    {
        Vector2 randomLocationRunaway = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-1.5f, 1.5f));
        GenericSeekerComponent.StartPath(GenericRigidBodyComponent.position, randomLocationRunaway, OnRandomPathBuilt);
    }

    void OnRandomPathBuilt(Path TempPath)
    {
        if (!TempPath.error)
        {
            float currentKnightDistance = Vector2.Distance(GenericRigidBodyComponent.position, KnightScriptComponenet.KnightRigidBodyComponent.position);
            float proposedKnightDistance = Vector2.Distance(TempPath.vectorPath[1], KnightScriptComponenet.KnightRigidBodyComponent.position);

            if (proposedKnightDistance > currentKnightDistance || (Random.Range(0, RunawayRandomAmount) + currentKnightDistance) > RunawayRandomAmount * 2)
            {
                pathToFollow = TempPath;
                currentWayPointNumber = 0;
            }
            else if (SafetyTryNumber < 10)
            {
                FindRunawayPath();
                SafetyTryNumber++;
            }

            SafetyTryNumber = 0;
        }
    }

    void OnPathBuilt(Path TempPath)
    {
        if (!TempPath.error)
        {
            pathToFollow = TempPath;
            currentWayPointNumber = 0;
        }
    }


    void FixedUpdate()
    {
        float PoistionOfAimX = 0;

        if (Aim == PathingAim.ChaseKnight || Aim == PathingAim.RunawayFromKnight)
        {
            PoistionOfAimX = KnightScriptComponenet.KnightRigidBodyComponent.position.x;
        }
        else if (Aim == PathingAim.ChaseAnotherObject)
        {
            PoistionOfAimX = ObjectToChase.GetComponent<Rigidbody2D>().position.x;
        }

        //FACE PLAYER CODE
        if (GenericRigidBodyComponent.position.x > PoistionOfAimX) //Knight is to the Left
        {
            if ((lookingRight && shouldFaceAim) || (!lookingRight && !shouldFaceAim))
            {
                transform.RotateAround(transform.position, transform.up, 180f);
                lookingRight = !lookingRight;
            }
        }
        else if (GenericRigidBodyComponent.position.x < PoistionOfAimX) //Knight is to the Right
        {
            if ((!lookingRight && shouldFaceAim) || (lookingRight && !shouldFaceAim))
            {
                transform.RotateAround(transform.position, transform.up, 180f);
                lookingRight = !lookingRight;
            }
        }

        //PATHFINDING CODE
        if (!EnemyMovementStats_Interface.HitCurrentlyCantMove && !immediateStop)
        {
            if (pathToFollow != null && (currentWayPointNumber < pathToFollow.vectorPath.Count) && !AimHasChanged)
            {
                TryToFollowPath();
            }
        }

    }

    private void TryToFollowPath()
    {

        Vector2 CurrentPathPoint = pathToFollow.vectorPath[currentWayPointNumber];
        Vector2 distanceBetween = CurrentPathPoint - GenericRigidBodyComponent.position;
        Vector2 direction = distanceBetween.normalized;

        bool movementSuccess = false;

        if (StuckSoRandomPathing || CanMoveThroughEnemies)
        { movementSuccess = TryMoveVaryingSpeeds(direction, 0); } //No mask filtering
        else
        { movementSuccess = TryMoveVaryingSpeeds(direction, 128); } //64 = walls, 128 = enemies

        // if (!movementSuccess || GenericRigidBodyComponent.position == PreviousLocation)
        if (movementSuccess)
        { ActivateTimerOnBool(0.2f, "RecentlyMovedSucessfully"); }


        // PreviousLocation = ThiefRigidBodyComponent.position;

        if (!RecentlyMovedSucessfully && !StuckSoRandomPathing)
        {
            randomLocation.x = Random.Range(-2, 2);
            randomLocation.y = Random.Range(-1, 1);

            ActivateTimerOnBool(0.4f, "StuckSoRandomPathing");
        }

        float distance = Vector2.Distance(GenericRigidBodyComponent.position, CurrentPathPoint);
        if (distance <= 0.1)
        {
            currentWayPointNumber++;
        }
    }

    private bool TryMoveVaryingSpeeds(Vector2 direction, int layerMaskInt)
    {
        double movementSpeed = EnemyMovementStats_Interface.MovementSpeed;
        int iterations = 4; //(CLUMP LEVEL OF ENEMIES)
        bool successMovement = false;
        while (!successMovement && iterations > 0)
        {
            successMovement = TryMoveOneSpeed(direction, layerMaskInt, movementSpeed);
            iterations--;
            movementSpeed = movementSpeed / 2;
        }

        return successMovement;
    }

    private bool TryMoveOneSpeed(Vector2 direction, int layerMaskInt, double movementSpeedDub)
    {
        List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(layerMaskInt);
        contactFilter2D.useTriggers = true;
        float movementSpeed = (float)movementSpeedDub;

        int castCount = GenericRigidBodyComponent.Cast(
                direction,
                contactFilter2D,
                castCollisions,
                movementSpeed * Time.fixedDeltaTime
                );

        if (castCount == 0)
        {
            GenericRigidBodyComponent.MovePosition(GenericRigidBodyComponent.position + direction * movementSpeed * Time.fixedDeltaTime);
            return true;

        }
        else
        { return false; }

    }



}
