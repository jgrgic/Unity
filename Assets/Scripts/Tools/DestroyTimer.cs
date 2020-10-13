using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A timer that destroys this gameobject.
public class DestroyTimer : MonoBehaviour {
    public float time;

    // Called every frame.
    private void Update() {
        time -= Time.deltaTime;
        if (time <= 0) {
            Destroy(gameObject);
        }
    }
}
