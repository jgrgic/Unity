using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drift : MonoBehaviour {
    public Vector3 direction;
    public float speed;

    // Called every frame.
    private void Update() {
        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}
