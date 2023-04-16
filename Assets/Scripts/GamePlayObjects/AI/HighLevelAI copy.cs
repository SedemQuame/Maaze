using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

///<summary>
/// AI implementation to help player enemies explore the enviroment.
///</summary>
public class HighLevelAI : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public enum ENEMY_AI_STATES { PATROL = 0, CHASE = 1, ATTACK = 2, DEAD = 3 };
    public float minimumAttackDistance;
    public float fieldOfView;
    public GameObject face;
    public GameObject projectilePrefab;


    private Transform player;
    private string destinationCell;
    private Vector3 patrolDestination;
    private Vector3 moveDirection;

    [Tooltip("Sets the initial state for enemy objects")]
    [SerializeField]
    private ENEMY_AI_STATES currentState;
    private MazeLoader loader;
    private bool isDestinationSet;

    private bool isGameOver;
    private GameObject mazeLoader;


    void Start()
    {
        isDestinationSet = false;
        currentState = ENEMY_AI_STATES.PATROL;
        if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player").transform;
        }
        isGameOver = GameObject.Find("Manager").GetComponent<GameManager>().isGameOver;

        mazeLoader = GameObject.Find("Maze Loader Holder");
    }

    void Update()
    {
        if (!isGameOver)
        {
            FSM();
        }
    }

    void FSM()
    {
        // implementation of the FSM to change player actions in response to change in state
        switch (currentState)
        {
            case ENEMY_AI_STATES.PATROL:
                patrolState();
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

    public void changeFSMState(ENEMY_AI_STATES value)
    {
        currentState = value;
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
            if ((agent.position - target.position).sqrMagnitude > minimumAttackDistance) return false;

            //Can be seen (the enemy (agent) can see the player (target)
            return true;
        }
        return false;
    }

    Vector3 randomPatrolDestination(string cell)
    {
        return GameObject.Find(cell).transform.position;
    }

    bool checkIfAtDestination(Vector3 targetDestination, float minimumDistanceToTarget = 1)
    {
        // // check to see if there is no gameObject between gameObject and destination.
        // Debug.Log("Vector3 distance between transform = " + transform.position + " and destination = " + destination + " is: " + Vector3.Distance(transform.position, destination));
        // Debug.Log("Distance for comparing is: " + distance);
        return (Vector3.Distance(transform.position, targetDestination) <= minimumDistanceToTarget);
    }

    public void patrolState()
    {
        if (!isDestinationSet)
        {
            loader = mazeLoader.GetComponent<MazeLoader>();

            destinationCell = "Floor " + Random.Range(0, loader.getRowAndColumnNumber()) + "," + Random.Range(0, loader.getRowAndColumnNumber());
            patrolDestination = randomPatrolDestination(destinationCell);
            navMeshAgent.SetDestination(patrolDestination);

            if (player.transform != null && checkIfAtDestination(player.transform.position, 2.5f))
            {
                changeFSMState(ENEMY_AI_STATES.CHASE);
            }
        }
    }

    public void chaseState()
    {
        // Debug.Log("In the chase state");
        // set new destination to the player's location.
        if (!isGameOver)
        {
            navMeshAgent.SetDestination(player.transform.position);

            // if the magnitude of the distance between the player and the enemy
            // is within the attack range, then transition to the attack state.
            if (checkIfAtDestination(patrolDestination, 1.5f) && spotPlayer(this.transform, player.transform))
            {
                // transition to the attack state of the F.S.M
                changeFSMState(ENEMY_AI_STATES.ATTACK);
            }
            // else if the magnitude of the distance between the player and the enemy
            // is x times greater than the attack range, then transition to the patrol state. 
            else if (checkIfAtDestination(patrolDestination, 3.5f)) // replace magic number with variable name
            {
                // transition to the patrol state of the F.S.M
                changeFSMState(ENEMY_AI_STATES.PATROL);
            }
        }
    }

    public void attackState()
    {
        // Debug.Log("In the attack state");
        // the attack state would work, by firing projectiles in the direction of the player, till it dies, or is out of firing distance.
        // the various enemy prefabs, should be made to shoot different types of projectiles.
        // make sure that the player can rotate.
        // play shooting particle system

        if (checkIfAtDestination(player.transform.position, minimumAttackDistance) || player.GetComponent<PlayerController>().health > 0)
        {
            Transform bulletProjectile = Instantiate(projectilePrefab.transform, face.transform.position, face.transform.rotation);
            Vector3 shootDir = (face.transform.position - player.position).normalized;
            bulletProjectile.GetComponent<BulletBehaviour>().Setup(shootDir);

            // if the enemy is killed by the player transition to the Dead state.
            if (transform.GetComponent<EnemyController>().health < 1)
            {
                // transition to the dead state of the F.S.M
                changeFSMState(ENEMY_AI_STATES.DEAD);
            }
        }
        else
        {
            // transition to the chase state of the F.S.M
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
}
