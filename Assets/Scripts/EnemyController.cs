using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


enum EnemyState {
    RandomWalk,
    Pursuit, 
    Patrol
}

public class EnemyController : MonoBehaviour {
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
        state = PatrolOrRandomWalk();
        agent = GetComponent<NavMeshAgent>();
        sinceLastChange = 0;
        destination = transform.position;
    }

    float sinceLastChange;
    float sinceLastDetection;

    float lastSeen = 0;
    public float memoryInSeconds= 3;

    public List<Vector3> patrolPoints;
    Vector3? currentPatrolPoint = null;


    bool InLineOfSight(Vector3 castTo) {
        Vector3 direction = castTo - transform.position;
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, direction, out hit)) {
            //Debug.DrawRay(transform.position, direction, Color.blue);
            return hit.collider.transform == this.target;
        } else {
            //Debug.DrawRay(transform.position, direction, Color.red);
            return false;
        }
    }

    EnemyState PatrolOrRandomWalk() {
        if (Random.value < 0.5f)
            return EnemyState.Patrol;
        else
            return EnemyState.RandomWalk;
    }

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

            case EnemyState.Patrol:
                if(currentPatrolPoint == null) {
                    currentPatrolPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
                }

                if(Vector3.Distance(transform.position, (Vector3)currentPatrolPoint) <= 0.1f) {
                    int currIndex = patrolPoints.IndexOf((Vector3)currentPatrolPoint);
                    int nextIndex = (currIndex + 1) % patrolPoints.Count;
                    currentPatrolPoint = patrolPoints[nextIndex];
                }
                agent.SetDestination((Vector3)currentPatrolPoint);

                break;
		}

        // state change
        if (sinceLastDetection <= 0) {
            sinceLastDetection = detectionInterval;
            if (Vector3.Distance(target.position, transform.position) <= detectionRange
                && InLineOfSight(target.position)
                ) {
                lastSeen = Time.time;
                state = EnemyState.Pursuit;
            } else if (state==EnemyState.Pursuit) {
                if(Time.time-lastSeen>=memoryInSeconds) {
                    state = PatrolOrRandomWalk();
                }
			}
        }
    }
}
