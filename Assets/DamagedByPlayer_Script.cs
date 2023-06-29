using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedByPlayer_Script : Utility_Functions
{
    BasicItemStats_Interface BasicItemStats_Interface;
    EnemyMovementStats_Interface EnemyMovementStats_Interface;
    CommonVariables_Interface CommonVariables_Interface;
    SpriteRenderer GenericSpriteRendererComponent;
    Color CurrentColorLevel;
    public int startingHealth;
    float startingSpeed;

    private bool _isRed;
    public bool IsRed
    {
        get { return this._isRed; }
        set
        {
            this._isRed = value;
            if (_isRed)
            {
                Color RedColor = new Color(1.0f, 0.0f, 0.0f, 0.7f);
                GenericSpriteRendererComponent.color = RedColor;
                SetAllChildrenToColor(RedColor);


            }
            else
            {
                GenericSpriteRendererComponent.color = CurrentColorLevel;
                SetAllChildrenToColor(CurrentColorLevel);
            };
        }
    }



    void Start()
    {
        GenericSpriteRendererComponent = GetComponent<SpriteRenderer>();
        BasicItemStats_Interface = GetComponent<BasicItemStats_Interface>();
        EnemyMovementStats_Interface = GetComponent<EnemyMovementStats_Interface>();
        CommonVariables_Interface = GetComponent<CommonVariables_Interface>();

        startingHealth = BasicItemStats_Interface.Health;

        if (EnemyMovementStats_Interface != null)
        {
            startingSpeed = EnemyMovementStats_Interface.MovementSpeed;
        }


        CurrentColorLevel = GenericSpriteRendererComponent.color;
    }

    void FixedUpdate()
    {
        if (BasicItemStats_Interface.Health == 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage()
    {
        BasicItemStats_Interface.Health--;

        if (!IsRed)
        {
            ActivateTimerOnBool(0.15f, "IsRed");
        }

        float HealthRatio = (float)BasicItemStats_Interface.Health / (float)startingHealth;
        float lessChange = (HealthRatio / 2) + 0.5f;

        CurrentColorLevel = new Color(1, 1, 1, lessChange);

        if (EnemyMovementStats_Interface != null)
        {
            CommonVariables_Interface.ActivateTimerOnBool(EnemyMovementStats_Interface.AmountOfTimeStoppedAfterHit, "HitCurrentlyCantMove");

            float divisionNumber = 1 - EnemyMovementStats_Interface.PercentageOfSpeedLostWhenHit;
            EnemyMovementStats_Interface.MovementSpeed = EnemyMovementStats_Interface.MovementSpeed * divisionNumber;
        }
    }

    public void HealToMax()
    {
        BasicItemStats_Interface.Health = startingHealth;
        CurrentColorLevel = new Color(1, 1, 1, 1);
        GenericSpriteRendererComponent.color = CurrentColorLevel;
        SetAllChildrenToColor(CurrentColorLevel);
        EnemyMovementStats_Interface.MovementSpeed = startingSpeed;
    }


    void SetAllChildrenToColor(Color color)
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.tag != "Arrow")
            {
                SpriteRenderer temp = child.gameObject.GetComponent<SpriteRenderer>();
                if (temp != null)
                {
                    temp.color = color;

                }

            }
        }
    }
}
