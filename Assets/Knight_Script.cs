using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Knight_Script : Utility_Functions
{
    //Components
    InputAction FireAction;
    public Rigidbody2D KnightRigidBodyComponent;
    Animator KnightAnimatorComponent;
    SpriteRenderer KnightSpriteRendererComponent;
    PlayerInput KnightPlayerInputComponent;
    Collider2D KnightCollider;
    // End Components
    public GameObject NinjastarRef;
    public GameObject ArrowRef;

    Vector2 MovementInput;
    Color DamageFlashColor = new Color32(255, 51, 0, 255); //SET HERE
    int Health = 5; //SET HERE
    Vector2 damagedByDirection;
    Vector2 moveLeftandRight = new Vector2(1, 0);


    public bool shouldstop { get; set; } = false; // C# 6 or higher
    private bool _isInvulnerable = false;
    public bool IsInvulnerable
    {
        get { return this._isInvulnerable; }
        set
        {
            this._isInvulnerable = value;
            if (_isInvulnerable)
            { KnightSpriteRendererComponent.color = new Color(1, 1, 1, 0.4f); }
            else
            { KnightSpriteRendererComponent.color = new Color(1, 1, 1, 1); }
        }
    }

    private bool _damageFlashingOccuring = false;
    public bool DamageFlashingOccuring
    {
        get { return this._damageFlashingOccuring; }
        set
        {
            this._damageFlashingOccuring = value;
            if (_damageFlashingOccuring)
            {
                CancelInvoke("ShootArrow");
                InvokeRepeating("FlashDamagedColor", 0, 0.2f);
                InvokeRepeating("DamageMovement", 0f, 0.1f);
            }
            else
            {
                CancelInvoke("FlashDamagedColor");
                CancelInvoke("DamageMovement");

                ActivateTimerOnBool(1.5f, "IsInvulnerable");
            }
        }
    }
    public bool ExperiencingKnockBack = false;
    public bool BowBeenStolen = false;
    bool lookingRight = true;
    bool IsShooting = false;

    bool UpgradeShield = false;


    private bool _upgradeBow = false;
    public bool UpgradeBow
    {
        get { return this._upgradeBow; }
        set { this._upgradeBow = value; SetChildActive(value, "KnightBow"); }
    }

    void FlashDamagedColor()
    {
        if (KnightSpriteRendererComponent.color == DamageFlashColor)
        { KnightSpriteRendererComponent.color = new Color(1, 1, 1, 1); }
        else
        { KnightSpriteRendererComponent.color = DamageFlashColor; }
    }


    private void OnTriggerEnter2D(Collider2D otherThing)
    {
        CommonVariables_Interface CommonVariables_Interface = otherThing.GetComponent<CommonVariables_Interface>();

        if (CommonVariables_Interface != null && CommonVariables_Interface.DamagesPlayerOnContact == true && !IsInvulnerable && !DamageFlashingOccuring)
        {

            bool enemyIsToTheRight = false;
            if (otherThing.gameObject.GetComponent<Rigidbody2D>().position.x > KnightRigidBodyComponent.position.x)
            {
                enemyIsToTheRight = true;
            }


            if (UpgradeShield && ((enemyIsToTheRight & lookingRight) || (!enemyIsToTheRight && !lookingRight)))
            {
                damagedByDirection = (KnightRigidBodyComponent.position - otherThing.gameObject.GetComponent<Rigidbody2D>().position).normalized;
                ActivateTimerOnBool(0.03f, "ExperiencingKnockBack");
            }
            else
            {
                Health--;
                KnightAnimatorComponent.SetBool("isKnightMoving", true);
                ActivateTimerOnBool(1f, "DamageFlashingOccuring");
                ActivateTimerOnBool(0.1f, "ExperiencingKnockBack");
            }
        }

        Thief_Script Thief_Script = otherThing.GetComponent<Thief_Script>();
        if (Thief_Script)
        {

        }

        if (otherThing.tag == "Upgrade" && otherThing.name == "BowUpgrade")
        {
            UpgradeBow = true;
            Destroy(otherThing.gameObject);
        }

        if (otherThing.name == "Ladder")
        {

            int CurrentSceneNumber = SceneManager.GetActiveScene().buildIndex;

            if (CurrentSceneNumber < SceneManager.sceneCount)
            {
                SceneManager.LoadScene(CurrentSceneNumber + 1);
            }
            Debug.LogWarning("LADDER TOUCHED!!");

        }

    }
    void DamageMovement()
    {
        //4 then 2f
        float moveSpeed = 1f; //push back strength
        moveLeftandRight.x = moveLeftandRight.x * -1;
        TryMove(moveLeftandRight, moveSpeed);
    }

    void Start()
    {
        //Components
        KnightRigidBodyComponent = GetComponent<Rigidbody2D>();
        KnightAnimatorComponent = GetComponent<Animator>();
        KnightSpriteRendererComponent = GetComponent<SpriteRenderer>();
        KnightPlayerInputComponent = GetComponent<PlayerInput>();
        KnightCollider = GetComponent<Collider2D>();
        // End Components

        FireAction = KnightPlayerInputComponent.actions["Fire"];
        FireAction.started += FireActionPressed;
        FireAction.canceled += FireActionReleased;

        UpgradeBow = true;




    }
    void OnDisable()
    {
        FireAction.started -= FireActionPressed;
        FireAction.canceled -= FireActionReleased;
    }
    public void FireActionPressed(InputAction.CallbackContext context)
    {
        if (!IsInvulnerable && !DamageFlashingOccuring)
        {
            InvokeRepeating("ShootArrow", 0f, 0.2f);
            IsShooting = true;
        }
    }
    void FireActionReleased(InputAction.CallbackContext context)
    {
        CancelInvoke("ShootArrow");
        IsShooting = false;
    }

    void OnMove(InputValue movementValue)
    {
        MovementInput = movementValue.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        // FACE DIRECTION OF ATTEMPTED PLAYER MOVEMENT
        // if (MovementInput.x < 0)
        // {
        //     KnightSpriteRendererComponent.flipX = true;
        // }
        // else if (MovementInput.x > 0)
        // {
        //     KnightSpriteRendererComponent.flipX = false;
        // }
        //

        if (MovementInput.x < 0 && lookingRight && !IsShooting) //Knight should look Left
        {
            transform.RotateAround(transform.position, transform.up, 180f);
            lookingRight = false; ;

        }
        else if (MovementInput.x > 0 && !lookingRight && !IsShooting) //Knight should look Right
        {
            transform.RotateAround(transform.position, transform.up, 180f);
            lookingRight = true;
        }

        if (!DamageFlashingOccuring && !ExperiencingKnockBack)
        {
            bool successOfMovement = false;
            float moveSpeed = 1f; //SET HERE
            if (MovementInput != Vector2.zero)
            {
                successOfMovement = TryMove(MovementInput, moveSpeed);
                if (!successOfMovement)
                {
                    successOfMovement = TryMove(new Vector2(MovementInput.x, 0), moveSpeed);

                    if (!successOfMovement)
                    {
                        successOfMovement = TryMove(new Vector2(0, MovementInput.y), moveSpeed);
                    }
                }
            }
            KnightAnimatorComponent.SetBool("isKnightMoving", successOfMovement);
        }

        if (ExperiencingKnockBack)
        {
            float moveSpeed = 2f; //push back strength
            TryMove(damagedByDirection, moveSpeed);
        }
    }

    private bool TryMove(Vector2 direction, float MovementSpeed)
    {
        List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.SetLayerMask(64);
        contactFilter2D.useTriggers = true;

        float CollisionOffSet = 0f;

        int castCount = KnightCollider.Cast(
                direction,
                contactFilter2D,
                castCollisions,
                MovementSpeed * Time.fixedDeltaTime + CollisionOffSet,
                true
                );

        if (castCount == 0)
        {
            KnightRigidBodyComponent.MovePosition(KnightRigidBodyComponent.position + direction * MovementSpeed * Time.fixedDeltaTime);
            return true;
        }
        else
        { return false; }
    }

    void OnFire()
    {
        // GameObject emptyGO = new GameObject();
        // Transform newTransform = new GameObject().transform;
        // newTransform.position = KnightRigidBodyComponent.position;
        // GameObject gameObjectNinjastar = Instantiate(NinjastarRef, newTransform);
        // gameObjectNinjastar.name = "Ninjastar";
        // Ninjastar_Script NinjastarScript = gameObjectNinjastar.GetComponent<Ninjastar_Script>();
        // Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Vector2 throwVector = (worldMousePosition - KnightRigidBodyComponent.position).normalized;

        // NinjastarScript.thrownDirection = throwVector;

    }

    void ShootArrow()
    {

        if (UpgradeBow)
        {
            Quaternion rotation = new Quaternion();
            GameObject go = Instantiate(ArrowRef, KnightRigidBodyComponent.position, rotation);
            go.name = "Arrow";
            //  gameObjectArrow.GetComponent<Rigidbody2D>().position = KnightRigidBodyComponent.position;
            if (KnightRigidBodyComponent.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x && lookingRight)
            {
                transform.RotateAround(transform.position, transform.up, 180f);
                lookingRight = false;
            }
            else if (KnightRigidBodyComponent.position.x < Camera.main.ScreenToWorldPoint(Input.mousePosition).x && !lookingRight)
            {
                transform.RotateAround(transform.position, transform.up, 180f);
                lookingRight = true;
            }
        }
    }

}
