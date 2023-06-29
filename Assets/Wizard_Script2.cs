using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class Wizard_Script2 : Utility_Functions, BasicItemStats_Interface, EnemyMovementStats_Interface, CommonVariables_Interface
{

    public int Health { get; set; } = 5;
    public float MovementSpeed { get; set; } = 1.2f;
    public float PercentageOfSpeedLostWhenHit { get; set; } = 0f;
    public int AmountOfTimeStoppedAfterHit { get; set; } = 1;

    //Components
    Knight_Script KnightScriptComponenet;
    Rigidbody2D WizardRigidBodyComponent;
    FollowPath_Script WizardFollowPath_Script;
    Seeker ThiefSeekerComponent;
    // End Components

    public GameObject UpgradeRef;

    bool CurrentlyHealing = false;


    void Start()
    {
        //Components
        KnightScriptComponenet = GameObject.FindGameObjectWithTag("Player").GetComponent<Knight_Script>();
        WizardRigidBodyComponent = GetComponent<Rigidbody2D>();
        WizardFollowPath_Script = GetComponent<FollowPath_Script>();
        // End Components

        WizardFollowPath_Script.shouldFaceAim = false;
        WizardFollowPath_Script.RunawayRandomAmount = 1;
        WizardFollowPath_Script.CanMoveThroughEnemies = true;

        // InvokeRepeating("UpdatePath", 0f, 0.3f);

    }



    void Update()
    {
        List<GameObject> EnemyObjectList = FindAllObjectsOfLayer("Enemy");


        Dictionary<GameObject, float> HurtEnemiesDict = new Dictionary<GameObject, float>();

        bool DictionaryNotEmpty = false;

        foreach (GameObject go in EnemyObjectList)
        {
            if (go.GetComponent<BasicItemStats_Interface>().Health < go.GetComponent<DamagedByPlayer_Script>().startingHealth && go != this.gameObject)
            {
                float distance = Vector2.Distance(WizardRigidBodyComponent.position, go.GetComponent<Rigidbody2D>().position);
                HurtEnemiesDict.Add(go, distance);
                DictionaryNotEmpty = true;
            }
        }


        if (DictionaryNotEmpty && !CurrentlyHealing)
        {
            var ClosestObject = (from entry in HurtEnemiesDict orderby entry.Value ascending select entry).First();

            if (ClosestObject.Value < 0.1)
            {
                CurrentlyHealing = true;

                ClosestObject.Key.GetComponent<DamagedByPlayer_Script>().HealToMax();
                IEnumerator newCoroutine = ChangeColorsBack(ClosestObject.Key);
                StartCoroutine(newCoroutine);
                foreach (GameObject go in ClosestObject.Key.GetComponent<CommonVariables_Interface>().GetChildrenByName("Arrow"))
                {
                    Destroy(go);
                }
            }
            else if (ClosestObject.Key != WizardFollowPath_Script.ObjectToChase)
            {

                WizardFollowPath_Script.Aim = FollowPath_Script.PathingAim.ChaseAnotherObject;
                WizardFollowPath_Script.shouldFaceAim = true;
                WizardFollowPath_Script.ObjectToChase = ClosestObject.Key;
            }

        }
        else if (!CurrentlyHealing)
        {
            WizardFollowPath_Script.Aim = FollowPath_Script.PathingAim.RunawayFromKnight;
            WizardFollowPath_Script.shouldFaceAim = false;
        }
    }

    private IEnumerator ChangeColorsBack(GameObject objectToChange)
    {


        SpriteRenderer rend = objectToChange.GetComponent<SpriteRenderer>();
        Color flashColor = Color.cyan;

        EnemyMovementStats_Interface EnemyMovementStatsnterface = objectToChange.GetComponent<EnemyMovementStats_Interface>();

        if (EnemyMovementStatsnterface != null)
        {
            EnemyMovementStatsnterface.HitCurrentlyCantMove = true;
        }

        GameObject Staff = GetChildrenByName("Staff")[0];
        SpriteRenderer rendStaff = Staff.GetComponent<SpriteRenderer>();





        for (int i = 0; i < 10; i++)
        {
            if (objectToChange != null)
            {
                if (rend.color == new Color(1, 1, 1, 1))
                {
                    rend.color = flashColor;
                    rendStaff.color = flashColor;
                }
                else
                {
                    rend.color = new Color(1, 1, 1, 1);
                    rendStaff.color = new Color(1, 1, 1, 1);

                }
            }
            yield return new WaitForSeconds(0.1f);

        }


        if (objectToChange != null)
        {
            rend.color = new Color(1, 1, 1, 1);
            rendStaff.color = new Color(1, 1, 1, 1);
            CurrentlyHealing = false;

            if (EnemyMovementStatsnterface != null)
            {
                EnemyMovementStatsnterface.HitCurrentlyCantMove = false;
            }
        }

    }


}
