using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    Camera cam;
    NavMeshAgent agent;
    public float changeInterval = 3;
    public Vector3 minBound = new Vector3(-10,1.25f,-10);
    public Vector3 maxBound = new Vector3(10, 1.25f, 10);
    Vector3 destination;

    public Transform target;
    public float detectionRange = 5;
    public float detectionInterval = .5f;

    void Start() {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        sinceLastChange = 0;
        destination = transform.position;
    }

    float sinceLastChange;
    float sinceLastDetection;
    bool inPursuit=false;
    void Update() {
        sinceLastChange -= Time.deltaTime;
        sinceLastDetection -= Time.deltaTime;

        if (!inPursuit && sinceLastChange <= 0) {
            sinceLastChange = changeInterval;

            destination= new Vector3(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.y, maxBound.y), Random.Range(minBound.z, maxBound.z));

            agent.SetDestination(destination);
        }

        if (sinceLastDetection <= 0) {
            sinceLastDetection = detectionInterval;
            if (Vector3.Distance(target.position, transform.position) <= detectionRange) {
                inPursuit = true;
            }
        }

        if(inPursuit)
            agent.SetDestination(target.position);
    }
}
