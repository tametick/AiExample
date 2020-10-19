using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    Camera cam;
    NavMeshAgent agent;
    public float changeInterval = 3;
    public Vector3 minBound = new Vector3(-10,1.25f,-10);
    public Vector3 maxBound = new Vector3(10, 1.25f, 10);
    Vector3 destination;

    void Start() {
        cam = Camera.main;
        agent = GetComponent<NavMeshAgent>();
        sinceLastChange = 0;
        destination = transform.position;
    }

    float sinceLastChange;
    void Update() {
        sinceLastChange -= Time.deltaTime;

		if (sinceLastChange <= 0) {
            sinceLastChange = changeInterval;

            destination= new Vector3(Random.Range(minBound.x, maxBound.x), Random.Range(minBound.y, maxBound.y), Random.Range(minBound.z, maxBound.z));

            Debug.Log($"new destination {destination.ToString()}");

            agent.SetDestination(destination);
        }
    }
}
