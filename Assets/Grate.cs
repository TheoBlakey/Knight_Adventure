using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grate : MonoBehaviour
{
    Animator GrateAnimator;
    SpriteRenderer Rend;
    bool ReadyToOpen = true;

    void Start()
    {
        GrateAnimator = GetComponent<Animator>();
        Rend = GetComponent<SpriteRenderer>();

    }


    void OnTriggerEnter2D(Collider2D otherThing)
    {
        if ((otherThing.gameObject.layer == LayerMask.NameToLayer("Enemy") || otherThing.gameObject.tag == "Player") && ReadyToOpen)
        {
            GrateAnimator.SetTrigger("GrateOpens");
            Invoke("FlashEnd", 0.5f);
            Rend.color = Color.grey;
            ReadyToOpen = false;
        }
    }

    void GrateHasOpened()
    {
        gameObject.layer = LayerMask.NameToLayer("WallsAndColliders");
        AstarPath.active.Scan();
        Invoke("TimerForGateClose", 5f);
    }

    void GrateHasClosed()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        AstarPath.active.Scan();
        ReadyToOpen = true;
    }

    void TimerForGateClose()
    {
        GrateAnimator.SetTrigger("GrateCloses");
    }

    void FlashEnd()
    {
        Rend.color = new Color(1, 1, 1, 1);
    }


}
