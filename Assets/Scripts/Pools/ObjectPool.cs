using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://learn.unity.com/tutorial/introduction-to-object-pooling#5ff8d015edbc2a002063971d
// https://gameprogrammingpatterns.com/object-pool.html for free list

public abstract class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    [SerializeField]
    private List<GameObject> pool;
    [SerializeField]
    private GameObject template;
    [SerializeField]
    private int copies;

    // Awake is called when the script instance is loaded - it is the first
    // life-cycle event that gets called upon program initialisation. See:
    // https://docs.unity3d.com/Manual/ExecutionOrder.html
    protected void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        pool = new List<GameObject>();
        GameObject poolObject;

        for (int i = 0; i < copies; i++)
        {
            poolObject = Instantiate(template);
            poolObject.SetActive(false);
            pool.Add(poolObject);
        }
    }

    public GameObject RequestPoolObject()
    {
        // TODO LATER: create free list
        // See https://gameprogrammingpatterns.com/object-pool.html (also linked above)
        for (int i = 0; i < copies; i++)
        {
            if (!pool[i].activeInHierarchy)
                return pool[i];
        }

        return null;
    }
}
