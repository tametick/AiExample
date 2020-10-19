using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
    Camera cam;
    NavMeshAgent agent;

    void Start() {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                agent.SetDestination(hit.point);
            }

        }
    }
}