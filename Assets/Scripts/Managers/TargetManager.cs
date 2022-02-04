using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpdateAction
{
    Add,
    Remove
}

public class TargetManager : MonoBehaviour
{
    /// <summary>
    /// Master list of all targets in the arena.
    /// </summary>
    [SerializeField]
    private List<GameObject> targets;

    [SerializeField]
    private GameObject spawnManager;
    private SpawnManager spawnManagerScript;

    public IEnumerable Targets
    {
        get
        {
            return targets;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.StartListening(Events.TargetMatch, ClearTargetMatch);

        spawnManagerScript = spawnManager.GetComponent<SpawnManager>();
    }

    public void UpdateDestructibleColourCountDict(UpdateAction action)
    {
        switch (action)
        {
            case UpdateAction.Add:
                Debug.Log("UpdateAction.Add");
                // Each Target initialises itself with a random colour and material.
                // Get the colours and update destructibleColourCountDict in SpawnManager.
                foreach (GameObject target in targets)
                {
                    spawnManagerScript.IncrementDestructibleColourCount(
                        target.GetComponent<Target>().Colour);
                }
                break;
            case UpdateAction.Remove:
                Debug.Log("UpdateAction.Remove");
                // Each Target initialises itself with a random colour and material.
                // Get the colours and update destructibleColourCountDict in SpawnManager.
                foreach (GameObject target in targets)
                {
                    spawnManagerScript.DecrementDestructibleColourCount(
                        target.GetComponent<Target>().Colour);
                }
                break;
        }
    }

    private void ClearTargetMatch()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            if (targets[i].GetComponent<Target>().Matched)
            {
                GameObject remove = targets[i];

                // Update destructibleColourCountDict in SpawnManager.
                spawnManagerScript.DecrementDestructibleColourCount(
                    remove.GetComponent<Target>().Colour);

                targets.RemoveAt(i);
                Destroy(remove);
            }
        }
    }
}
