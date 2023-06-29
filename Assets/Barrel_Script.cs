using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Script : Utility_Functions, BasicItemStats_Interface
{
    private int _health = 3;
    public int Health
    {
        get
        { return this._health; }
        set
        {
            this._health = value;
            if (_health == 0)
            {
                foreach (Transform child in this.transform)
                { child.gameObject.layer = LayerMask.NameToLayer("Default"); }
                gameObject.layer = LayerMask.NameToLayer("Default");
                AstarPath.active.Scan();
            }
        }
    }

    void Start()
    {

    }
    void FixedUpdate()
    {

    }

}
