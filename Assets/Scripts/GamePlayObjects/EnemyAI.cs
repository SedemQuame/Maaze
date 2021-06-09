using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

///<summary>
/// AI implementation to help player enemies explore the enviroment.
///</summary>
public class EnemyAI : MonoBehaviour
{
    // public Camera camera;
    public NavMeshAgent navMeshAgent;
    public enum ENEMY_AI_STATES { PATROL = 0, OBSERVE = 1, CHASE = 2, ATTACK = 3, DEAD = 4 };
    public float attackRange;
    public float fieldOfView;
    public GameObject face;
    public GameObject projectilePrefab;

    private float nearDestinationDistance = 0.2f;

    private Transform player;
    private Vector3 moveDirection;
    private bool destinationIsSet;
    private string destinationCell;
    private int level = LevelDifficulty.levelDifficulty;
    private Vector3 patrolDestination;

    [Tooltip("Sets the initial state for enemy objects")]
    [SerializeField]
    private ENEMY_AI_STATES currentState;

    void Start()
    {
        destinationIsSet = false;
        currentState = ENEMY_AI_STATES.PATROL;
        player = GameObject.Find("Player").transform;
    }

    void FixedUpdate()
    {
        // implementation of the FSM to change player actions in response to change in state
        switch (currentState)
        {
            case ENEMY_AI_STATES.PATROL:
                patrolState();
                break;

            case ENEMY_AI_STATES.OBSERVE:
                observeState();
                break;

            case ENEMY_AI_STATES.CHASE:
                chaseState();
                break;

            case ENEMY_AI_STATES.ATTACK:
                attackState();
                break;

            case ENEMY_AI_STATES.DEAD:
                deadState();
                break;
        }
    }

    public bool spotPlayer(Transform agent, Transform target)
    {
        //Determine if player is within field of view 
        Vector3 vecDiff = target.position - agent.position;

        //Get angle between look at direction and player direction from enemy 
        float dot = Vector3.Dot(agent.forward.normalized, vecDiff.normalized);

        //If player is behind enemy, then exit 
        if (dot < 0) return false;

        //If player is not within viewing angle then exit 
        if (fieldOfView < (90f - dot * 90f)) return false;

        //Enemy is facing player. Is player within range and is there a direct line? 
        RaycastHit hit;
        if (Physics.Raycast(agent.position, target.position, out hit, -1))
        {
            //Has direct line, is within range? 
            if ((agent.position - target.position).sqrMagnitude > attackRange) return false;

            //Can be seen (the enemy (agent) can see the player (target)
            return true;
        }
        return false;
    }

    public void changeFSMState(ENEMY_AI_STATES value)
    {
        currentState = value;
    }

    public void patrolState()
    {
        // check if destination is set.
        // if destination not set => setDestionation for navMeshAgent
        // check to see if navMeshAgent is near destination
        // how do we check if the GameObject has reached it's destination?
        // once we can test that feature how do we make it such that it will choose and new patrol destination again?

        // if the destination cell has not been set, do it.
        if (!destinationIsSet)
        {
            GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
            MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();
            destinationCell = "Floor " + Random.Range(0, loader.getRowAndColumnNumber()) + "," + Random.Range(0, loader.getRowAndColumnNumber());
            Debug.Log("Destination cell: " + destinationCell);
            patrolDestination = randomPatrolDestination(destinationCell);
            navMeshAgent.SetDestination(patrolDestination);
            destinationIsSet = true;
        }

        Debug.Log("Near patrol destination:" + checkIfAtDestination(patrolDestination));

        // // check if the gameobject is close to the destination cell.
        // if (checkIfAtDestination(patrolDestination))
        // {
        //     // transition to the observe state of the AI.
        //     changeFSMState(ENEMY_AI_STATES.OBSERVE);
        // }
    }

    bool checkIfAtDestination(Vector3 destination)
    {
        // check to see if there is no gameObject between gameObject and destination.
        return (Vector3.Distance(transform.position, destination) >= nearDestinationDistance);
    }

    public void observeState()
    {
        Debug.Log("In the observe state");
        // if we see the player, transition to the chase state
        if (checkIfAtDestination(player.transform.position))
        {
            // fire a raycast to find out there is a wall between the enemy and player.
            // shoot ray forward, if intercepted by wall, change to patrol state.

            var ray = new Ray(this.transform.position, this.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 4))
            {
                if (hit.transform.gameObject.CompareTag("Wall"))
                {
                    changeFSMState(ENEMY_AI_STATES.PATROL);
                }
                else if (hit.transform.gameObject.CompareTag("Player"))
                {
                    changeFSMState(ENEMY_AI_STATES.CHASE);
                }
            }
        }

        changeFSMState(ENEMY_AI_STATES.PATROL);
    }
    public void chaseState()
    {
        Debug.Log("In the chase state");
        // set new destination to the player's location.
        navMeshAgent.SetDestination(player.transform.position);

        // if the magnitude of the distance between the player and the enemy
        // is within the attack range, then transition to the attack state.
        if (Vector3.Distance(transform.position, patrolDestination) <= attackRange && spotPlayer(this.transform, player.transform))
        {
            // transition to the attack state of the F.S.M
            changeFSMState(ENEMY_AI_STATES.ATTACK);
        }
        // else if the magnitude of the distance between the player and the enemy
        // is x times greater than the attack range, then transition to the observe state. 
        else if (Vector3.Distance(transform.position, patrolDestination) >= (attackRange + 5)) // replace magic number with variable name
        {
            // transition to the observe state of the F.S.M
            changeFSMState(ENEMY_AI_STATES.OBSERVE);
        }
    }

    public void attackState()
    {
        Debug.Log("In the attack state");
        // the attack state would work, by firing projectiles in the direction of the player, till it dies, or is out of firing distance.
        // the various enemy prefabs, should be made to shoot different types of projectiles.
        // make sure that the player can rotate.
        // play shooting particle system

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange || player.GetComponent<PlayerController>().health > 0)
        {
            Transform bulletProjectile = Instantiate(projectilePrefab.transform, face.transform.position, face.transform.rotation);
            Vector3 shootDir = (face.transform.position - player.position).normalized;
            bulletProjectile.GetComponent<BulletBehaviour>().Setup(shootDir);

            // if the enemy is killed by the player transition to the Dead state.
            if (transform.GetComponent<EnemyController>().health < 1)
            {
                // transition to the observe state of the F.S.M
                changeFSMState(ENEMY_AI_STATES.CHASE);
            }
        }
        else
        {
            // transition to the observe state of the F.S.M
            changeFSMState(ENEMY_AI_STATES.CHASE);
        }
    }

    public void deadState()
    {
        // add body here

        // play a particle effect that shows that the enemy is dead.
        // Destroy the game object.
        // transfer some game points to the player (this can be power up, health points etc)

        Destroy(transform.gameObject);
    }

    Vector3 randomPatrolDestination(string destinationCell)
    {
        return GameObject.Find(destinationCell).transform.position;
    }
}
