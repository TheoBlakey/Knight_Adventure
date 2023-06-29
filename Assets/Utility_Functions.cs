using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility_Functions : MonoBehaviour
{

    //if utilises commonVariables
    public bool DamagesPlayerOnContact { get; set; } = false;
    public bool CausesDamageToOthersOnContact { get; set; } = false;
    // public bool StopsPlayerFromMoving { get; set; } = false;

    //if utlites EnemyMovementStats_Interface
    public bool HitCurrentlyCantMove { get; set; } = false;




    // private void Start()
    // {
    // BasicItemStats_Interface BasicItemStatsInterface = GetComponent<BasicItemStats_Interface>();
    // if (BasicItemStatsInterface != null)
    // {
    // MaxHealth = Health;
    // }
    // }

    IDictionary<string, IEnumerator> coroutinesDictionary = new Dictionary<string, IEnumerator>() { };
    public void ActivateTimerOnBool(float timeAmount, string boolName)
    {
        IEnumerator foundCoroutine;
        if (coroutinesDictionary.TryGetValue(boolName, out foundCoroutine))
        {
            StopCoroutine(foundCoroutine);
            coroutinesDictionary.Remove(boolName);
        }
        IEnumerator newCoroutine = TimerForABoolean(timeAmount, boolName);
        StartCoroutine(newCoroutine);
        coroutinesDictionary.Add(boolName, newCoroutine);
    }
    private IEnumerator TimerForABoolean(float waitTime, string booleanName)
    {
        var booleanProperty = this.GetType().GetProperty(booleanName);
        var booleanField = this.GetType().GetField(booleanName);

        if (booleanField != null)
        {
            booleanField.SetValue(this, true);
        }
        else
        {
            booleanProperty = this.GetType().GetProperty(booleanName);
            if (booleanProperty != null)
            {
                booleanProperty.SetValue(this, true);
            }
        }

        yield return new WaitForSeconds(waitTime);

        if (booleanField != null)
        {
            booleanField.SetValue(this, false);
        }
        else if (booleanProperty != null)
        {
            booleanProperty.SetValue(this, false);
        }
    }

    public void SetChildActive(bool ActiveStatus, string Name)
    {
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name == Name)
            {
                child.gameObject.SetActive(ActiveStatus);
            }
        }
    }

    public List<GameObject> GetChildrenByName(string Name)
    {
        List<GameObject> ObjectList = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.name == Name)
            {
                ObjectList.Add(child.gameObject);
            }
        }
        return ObjectList;
    }

    public List<GameObject> FindAllObjectsOfLayer(string layer)
    {
        GameObject[] AllGameObjects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
        List<GameObject> ObjectList = new List<GameObject>();
        foreach (GameObject go in AllGameObjects)
        {
            if (go.layer == LayerMask.NameToLayer(layer))
            {
                // Debug.Log(LayerMask.NameToLayer(layer));
                ObjectList.Add(go);
            }
        }
        return ObjectList;
    }




}