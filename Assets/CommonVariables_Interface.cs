using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CommonVariables_Interface
{
    bool DamagesPlayerOnContact
    { get; set; }
    bool CausesDamageToOthersOnContact
    { get; set; }
    // bool StopsPlayerFromMoving
    // { get; set; }


    void ActivateTimerOnBool(float timeAmount, string boolName)
    { }

    List<GameObject> GetChildrenByName(string Name)
    {
        return new List<GameObject>();
    }
}
