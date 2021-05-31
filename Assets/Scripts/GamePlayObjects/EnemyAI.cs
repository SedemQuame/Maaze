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
    public enum ENEMY_AI_STATES { PATROL = 0, CHASE = 1, ATTACK = 2, DEAD = 3 };
    private Transform player;
    public float attackDistance;
    private float minimumDistance = 3.0f;
    public float fieldOfView;
    private Vector3 moveDirection;
    private bool isDestinationSet;
    private string destinationCell;
    private int level = LevelDifficulty.levelDifficulty;
    private Vector3 floorDestination;

    [Tooltip("Sets the initial state for enemy objects")]
    [SerializeField]
    private ENEMY_AI_STATES currentState;

    void Start()
    {
        isDestinationSet = false;
        currentState = ENEMY_AI_STATES.PATROL;
        player = GameObject.Find("Player").transform;
    }

    Vector3 randomPatrolDestination(string destinationCell)
    {
        floorDestination = GameObject.Find(destinationCell).transform.position;
        return floorDestination;
    }

    void FixedUpdate()
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

            default:
                patrolState();
                break;
        }
    }

    public bool spotPlayer(Transform agent, Transform target, float attackDistance, float fieldOfView)
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
            if ((agent.position - target.position).sqrMagnitude > attackDistance) return false;

            //Can be seen (the enemy (agent) can see the player (target)
            return true;
        }
        return false;
    }

    //-------------------------------------------------- 
    //Draw forward vector of enemy for line of sight 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        moveDirection = transform.TransformDirection(Vector3.forward);
        Gizmos.DrawRay(transform.position, moveDirection);
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
        GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
        MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();
        destinationCell = "Floor " + Random.Range(0, loader.getRowAndColumnNumber()) + "," + Random.Range(0, loader.getRowAndColumnNumber());
        Debug.Log(destinationCell);
        if (Vector3.Distance(transform.position, floorDestination) <= minimumDistance || !isDestinationSet)
        {
            navMeshAgent.SetDestination(randomPatrolDestination(destinationCell));
            isDestinationSet = true;
        }
    }

    public void chaseState()
    {
        Debug.Log("In the chase state");
        // add body here
    }

    public void attackState()
    {
        // add body here
    }

    public void deadState()
    {
        // add body here
    }

    public void movement(GameObject collider)
    {
        // rotate in a given direction 
    }
}
