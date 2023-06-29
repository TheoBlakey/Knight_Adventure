using UnityEngine;

public class MazeCellObject : MonoBehaviour
{
    [System.NonSerialized]
    System.Collections.Generic.Stack<MazeCellObject> pool;

    public MazeCellObject GetInstance()
    {
        if (pool == null)
        {
            pool = new();
        }
        if (pool.TryPop(out MazeCellObject instance))
        {
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(this);
            instance.pool = pool;
        }
        return instance;
    }

    public void Recycle()
    {
        pool.Push(this);
        gameObject.SetActive(false);
    }
}
