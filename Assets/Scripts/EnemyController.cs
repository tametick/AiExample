using UnityEngine;
using UnityEngine.AI;


enum EnemyState {
    RandomWalk,
    Pursuit
}

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

    EnemyState state;

    void Start() {
        state = EnemyState.RandomWalk;
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        sinceLastChange = 0;
        destination = transform.position;
    }

    float sinceLastChange;
    float sinceLastDetection;

    float lastSeen = 0;
    public float memoryInSeconds= 3;

    void Update() {
        sinceLastChange -= Time.deltaTime;
        sinceLastDetection -= Time.deltaTime;

        // state action
		switch (state) {
            case EnemyState.RandomWalk:
                if (sinceLastChange <= 0) {
                    sinceLastChange = changeInterval;

                    destination = new Vector3(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.y, maxBound.y), Random.Range(minBound.z, maxBound.z));

                    agent.SetDestination(destination);
                }
                break;

            case EnemyState.Pursuit:
                agent.SetDestination(target.position);
                break;
		}

        // state change
        if (sinceLastDetection <= 0) {
            sinceLastDetection = detectionInterval;
            if (Vector3.Distance(target.position, transform.position) <= detectionRange) {
                lastSeen = Time.time;
                state = EnemyState.Pursuit;
            } else if (state==EnemyState.Pursuit) {
                if(Time.time-lastSeen>=memoryInSeconds) {
                    state = EnemyState.RandomWalk;
                }
			}
        }
    }
}
