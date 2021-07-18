using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: add marbles to array
    // account for both wave-spawned and projectiles
    [SerializeField]
    private List<GameObject> marbles;
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private Material[] marbleMaterials;
    [SerializeField]
    private GameObject player;
    private Cannon playerScript;
    [SerializeField]
    private GameObject projectileSpawner;
    private ProjectileSpawner projectileSpawnerScript;
    private int lives;
    private bool isActive = false;
    private Animation anim;
    //[SerializeField]
    //private int waves;
    //[SerializeField]
    //private int waveMarbleCount;

    // Start is called before the first frame update
    void Start()
    {
        lives = 3;
        playerScript = player.GetComponent<Cannon>();
        projectileSpawnerScript = projectileSpawner.GetComponent<ProjectileSpawner>();

        // Listen for MarbleMatch events upon game start.
        EventManager.StartListening(Events.MarbleMatch, HandleMarbleMatch);

        // Leverage coroutines to spawn marbles at regular intervals.
        foreach (GameObject spawnPoint in spawnPoints)
            StartCoroutine(SpawnMarbleInterval(spawnPoint.GetComponent<MarbleSpawner>(), 5, 0.25f));

        //player.SetActive(isActive);
        anim = player.GetComponent<Animation>();
    }

    // Added check for player.activeInHierarchy to prevent player input while
    // in the midst of respawning.

    // Player movement is handled in FixedUpdate() since physics are involved.
    private void FixedUpdate()
    {
        if (player.activeInHierarchy 
            && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)))
            playerScript.StrafeLeft();

        if (player.activeInHierarchy 
            && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)))
            playerScript.StrafeRight();

        // TODO REMOVE: debugging
        if (player.activeInHierarchy
            && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
            playerScript.StrafeUp();

        if (player.activeInHierarchy
            && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
            playerScript.StrafeDown();
    }

    // Player shooting is handled in Update() since responsiveness is important.
    //
    // Projectile physics are handled in their respective scripts'
    // FixedUpdate() methods.
    private void Update()
    {
        if (player.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            // Pass in a randomly-selected colour and its corresponding material
            // within GameManager to ensure array data is properly encapsulated.
            int marbleColour = GenerateMarbleColour();
            marbles.Add(
                projectileSpawnerScript.Shoot(
                    (Colours)marbleColour,
                    marbleMaterials[marbleColour]));
        }

        // TODO REMOVE: debugging
        //if (player.activeInHierarchy && Input.GetKeyDown(KeyCode.Space))
        //{
        //    //isActive = !isActive;
        //    //player.SetActive(isActive);
        //    //player.SetActive(false);
        //    //StartCoroutine(GreyOutPlayer(3f, player.GetComponent<MeshRenderer>().material.color));
        //    //StartCoroutine(playerScript.PlayResetSequence());
        //}

        // TODO REMOVE: debugging
        //if (player.activeInHierarchy && Input.GetKeyDown(KeyCode.F))
        //{
        //    //Extensions.ChangeRenderMode(player.GetComponent<MeshRenderer>().material, RenderModes.Transparent);
        //    //Color playerMaterialColour = player.GetComponent<MeshRenderer>().material.color;
        //    //playerMaterialColour.a -= 0.1f;
        //    //player.GetComponent<MeshRenderer>().material.color = playerMaterialColour;
        //    //player.GetComponent<Renderer>().enabled = !player.GetComponent<Renderer>().enabled;

        //    Extensions.ChangeRenderMode(player.GetComponent<MeshRenderer>().material, RenderModes.Transparent);
        //    Debug.Log("Begin Fade");
        //    anim.Play("Fade");
        //}
        //if (player.activeInHierarchy && Input.GetKeyDown(KeyCode.G))
        //{
        //    Debug.Log("Stop Fade");
        //    anim.Stop("Fade");
        //    Extensions.ChangeRenderMode(player.GetComponent<MeshRenderer>().material, RenderModes.Opaque);
        //}
    }

    private void HandlePlayerCollision()
    {
        if (--lives == 0)
            // Invoke game over. Do not need to check for lives in update, since
            // event chain can handle this.
            throw new System.NotImplementedException();
        else
        {
            // TODO LATER: particle effects
            player.SetActive(false);
            // start coroutine for timer
        }
    }

    /// <summary>
    /// Removes the specified marbles from the Scene.
    /// </summary>
    /// <param name="collider1">
    /// First marble to remove from the Scene.
    /// </param>
    /// <param name="collider2">
    /// Second marble to remove from the Scene.
    /// </param>
    private void HandleMarbleMatch(System.Object _collider1, System.Object _collider2)
    {
        GameObject collider1 = (GameObject)_collider1;
        GameObject collider2 = (GameObject)_collider2;

        //if (marbles.Contains(collider1) && marbles.Contains(collider2))
        //{
        //    marbles.Remove(collider1);
        //    marbles.Remove(collider2);
        //    Destroy(collider1);
        //    Destroy(collider2);
        //    // TODO LATER: increment score - maybe move this into another class
        //    // (scoring and combos can be handled within)
        //    //
        //    // Call that method using the event system.
        //}
    }

    /// <summary>
    /// Generates a random number corresponding to an index in materials.
    /// </summary>
    /// <returns>
    /// The result of the random number generation.
    /// </returns>
    private int GenerateMarbleColour()
    {
        return Random.Range(0, 0/*marbleMaterials.Length*/);
    }

    //private IEnumerator GreyOutPlayer(float duration, Color playerMaterialColour)
    //{
    //    yield return new WaitForSeconds(duration);
    //    player.SetActive(true);
    //    playerScript.GreyedOut = true;
    //    Extensions.ChangeRenderMode(player.GetComponent<MeshRenderer>().material, RenderModes.Transparent);
    //    playerMaterialColour.a = 0.3f;
    //    player.GetComponent<MeshRenderer>().material.color = playerMaterialColour;

    //}

    //private IEnumerator UngreyOutPlayer(float duration, Color playerMaterialColour)
    //{
    //    yield return new WaitForSeconds(duration);
    //    playerScript.GreyedOut = false;
    //    Extensions.ChangeRenderMode(player.GetComponent<MeshRenderer>().material, RenderModes.Opaque);
    //    playerMaterialColour.a = 1f;
    //    player.GetComponent<MeshRenderer>().material.color = playerMaterialColour;
    //}

    private IEnumerator PlayResetSequence(
        float duration, 
        bool greyedOut,
        RenderModes renderMode,
        Color playerMaterialColour,
        float alpha)
    {
        yield return new WaitForSeconds(duration);
        if (!player.activeInHierarchy) player.SetActive(true);
        playerScript.GreyedOut = greyedOut;
        Extensions.ChangeRenderMode(player.GetComponent<MeshRenderer>().material, renderMode);
        playerMaterialColour.a = alpha;
        player.GetComponent<MeshRenderer>().material.color = playerMaterialColour;

        if (playerScript.GreyedOut)
        {
            StartCoroutine(PlayResetSequence(
                3f,
                false,
                RenderModes.Opaque,
                player.GetComponent<MeshRenderer>().material.color,
                1f));
        }
    }

    /// <summary>
    /// Uses a coroutine to call marbleSpawner.SpawnMarble() at regular,
    /// specified intervals to spawn marbles a given number of times.
    /// </summary>
    /// <param name="marbleSpawner">
    /// Reference to a MarbleSpawner script.
    /// </param>
    /// <param name="repeats">
    /// The number of marbles to spawn.
    /// </param>
    /// <param name="interval">
    /// The frequency at which to spawn marbles.
    /// </param>
    /// <returns>
    /// Object of type IEnumerator - required for coroutine to work.
    /// </returns>
    private IEnumerator SpawnMarbleInterval(
        MarbleSpawner marbleSpawner, 
        int repeats, 
        float interval)
    {
        for (int i = 0; i < repeats; i++)
        {
            //List.Add(marbleSpawner.SpawnMarble());
            //GameObject marble = marbleSpawner.SpawnMarble();

            //MarbleMovement marbleScript = marble.GetComponent<MarbleMovement>();
            //Debug.Log(marbleScript.rb);
            //marbleScript.rb.velocity = marble.transform.position * marbleScript.speed;

            //marbles.Add(marble);
            
            // Note: yield first for now, since POC requires initial wave to be
            // called on Start(). Without the pause, dependencies i.e. the
            // RigidBody are not loaded yet, and NullReferenceExceptions get
            // thrown.
            // TODO LATER: Move wave spawning into Update()
            // Make sure that each wave is only spawned once - maybe via
            // conditional checks or something.
            yield return new WaitForSeconds(interval);

            // Pass in a randomly-selected colour and its corresponding material
            // within GameManager to ensure array data is properly encapsulated.
            int marbleColour = GenerateMarbleColour();
            marbles.Add(
                marbleSpawner.Spawn(
                    (Colours)marbleColour,
                    marbleMaterials[marbleColour]));
        }
    }
}

/*
    Issues with spawning:
    - you cannot spawn in rapid fire - there will be collisions and then the 
        marbles won't move in desired directions. So you should delay the actual
        spawning
    - What happens if, while spawning, the player destroys a marble?
        I was thinking of making spawning a 2-step process. In each spawner's
        script, a List<GameObject> of to-be-spawned marbles will be "buffered".
        In the script's Update(), new marbles will be instantiated at regular
        time intervals. Each time an instantiation occurs, the corresponding
        marble will be removed from the buffer and added to the GameManager's
        "master" List.
 */