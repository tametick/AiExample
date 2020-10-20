using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    List<EnemyController> enemies;

    void Start() {
        enemies = new List<EnemyController>(GetComponentsInChildren<EnemyController>());
		foreach (var e in enemies) {
            e.enemies = enemies;
		}
    }

}
