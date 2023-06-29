using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Script : Utility_Functions, CommonVariables_Interface
{
    Vector2 thrownDirection;
    public Rigidbody2D ArrowRigidBodyComponent;
    Rigidbody2D KnightRigidBodyComponent;
    SpriteRenderer ArrowSpriteRenderer;
    bool moving = true;
    public bool IsEvil = false;
    public Sprite SpriteEvilVersion;

    // void OnEnable()
    // {
    //     KnightRigidBodyComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    //     ArrowRigidBodyComponent = GetComponent<Rigidbody2D>();
    //     ArrowRigidBodyComponent.position = KnightRigidBodyComponent.position;
    // }

    void Start()
    {

        ArrowRigidBodyComponent = GetComponent<Rigidbody2D>();
        float AngleRad;

        if (IsEvil)
        {
            KnightRigidBodyComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
            thrownDirection = (KnightRigidBodyComponent.position - ArrowRigidBodyComponent.position).normalized;
            AngleRad = Mathf.Atan2(KnightRigidBodyComponent.position.y - ArrowRigidBodyComponent.position.y, KnightRigidBodyComponent.position.x - ArrowRigidBodyComponent.position.x);

            SpriteRenderer ArrowSpriteRenderer = GetComponent<SpriteRenderer>();
            ArrowSpriteRenderer.sprite = SpriteEvilVersion;
            DamagesPlayerOnContact = true;
        }
        else
        {
            Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            thrownDirection = (mouseScreenPosition - ArrowRigidBodyComponent.position).normalized;
            AngleRad = Mathf.Atan2(mouseScreenPosition.y - ArrowRigidBodyComponent.position.y, mouseScreenPosition.x - ArrowRigidBodyComponent.position.x);
        }

        float AngleDeg = (180 / Mathf.PI) * AngleRad;




        this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }

    // void Update()
    // {
    //     Debug.LogWarning("notconctime " + NonCollisionTime);
    //     if (NonCollisionTime > 0.0f)
    //     {
    //         NonCollisionTime -= Time.deltaTime;
    //     }

    // }

    void FixedUpdate()
    {
        if (moving == true)
        {
            float moveSpeed = 2.3f;
            ArrowRigidBodyComponent.MovePosition(ArrowRigidBodyComponent.position + (thrownDirection * moveSpeed) * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherThing)
    {

        if (moving)
        {

            DamagedByPlayer_Script PossibleDamagedByPlayerScript = otherThing.GetComponent<DamagedByPlayer_Script>();

            if (PossibleDamagedByPlayerScript != null && !IsEvil)
            {
                moving = false;
                this.transform.parent = otherThing.transform;
                PossibleDamagedByPlayerScript.TakeDamage();

            }
            else if (otherThing.gameObject.tag == "Player" && IsEvil)
            {
                if (!otherThing.gameObject.GetComponent<Knight_Script>().IsInvulnerable)
                {
                    moving = false;
                    this.transform.parent = otherThing.transform;
                    Invoke("StopBeingDeadly", 0.1f);
                    if (otherThing.gameObject.GetComponent<Knight_Script>().DamageFlashingOccuring)
                    {
                        Destroy(gameObject);
                    }
                }
            }
            else if (otherThing.gameObject.tag == "StopArrow")
            {
                moving = false;
                DamagesPlayerOnContact = false;
            }
            else if (IsEvil && otherThing.gameObject.name == "KnightShield")
            {
                moving = false;
                this.transform.parent = otherThing.transform;
                DamagesPlayerOnContact = false;

            }

        }


        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     if (moving == false)
        //     {
        //         // Destroy(gameObject);
        //     }

        // }
    }
    void StopBeingDeadly()
    {
        DamagesPlayerOnContact = false;
    }
}
