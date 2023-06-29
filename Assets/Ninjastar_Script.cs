using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninjastar_Script : MonoBehaviour
{
    public Vector2 thrownDirection;
    float moveSpeed = 2.25f;
    Rigidbody2D NinjaStarRigidBodyComponent;
    Collider2D NinjaStarColliderComponent;
    bool moving = true;
    // Start is called before the first frame update
    void Start()
    {
        NinjaStarRigidBodyComponent = GetComponent<Rigidbody2D>();
        NinjaStarColliderComponent = GetComponent<Collider2D>();
        int RandomNum = Random.Range(-100, 100);
        transform.Rotate(0, 0, RandomNum);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (moving == true)
        {
            transform.Rotate(0, 0, 700 * Time.deltaTime);
            NinjaStarRigidBodyComponent.MovePosition(NinjaStarRigidBodyComponent.position + (thrownDirection * moveSpeed) * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherThing)
    {

        if (otherThing.name == "Goblin")
        {
            Destroy(gameObject);
        }

        if (otherThing.name != "Knight" && otherThing.name != "Ninjastar")
        {
            moving = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (moving == false)
        {
            Destroy(gameObject);
        }

    }
}
