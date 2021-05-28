// using UnityEngine;
// using UnityEngine.AI;

// public class EnemyAI : MonoBehaviour
// {
//     private Vector3 startingPosition;
//     private Vector3 destinationPosition;
//     // private Camera camera;
//     private bool reachedSetDestination;
//     // public NavMeshAgent agent;
//     // Start is called before the first frame update
//     void Start()
//     {
//         camera = GameObject.Find("Main Camera").GetComponent<Camera>();

//         reachedSetDestination = false;
//         startingPosition = transform.position;
//         GameObject mazeLoader = GameObject.Find("Maze Loader Holder");
//         MazeLoader loader = mazeLoader.GetComponent<MazeLoader>();

//         string lastCreatedCell = "Floor " + (loader.getRowAndColumnNumber() - 1) + "," + (loader.getRowAndColumnNumber() - 1);
//         destinationPosition = GameObject.Find(lastCreatedCell).transform.position;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if(!reachedSetDestination){
//             Ray ray = camera.ScreenPointToRay(destinationPosition);
//             RaycastHit hit;
//             if(Physics.Raycast(ray, out hit)){
//                 // Move ai agent
//                 // agent.SetDestination(hit.point);
//             }
//         }
//     }
// }
