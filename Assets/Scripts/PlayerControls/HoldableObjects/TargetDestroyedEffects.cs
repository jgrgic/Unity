using System;
using System.Collections.Generic;
using UnityEngine;

// Makes special effects after a target is destroyed.
[RequireComponent(typeof(Target))]
public class TargetDestroyedEffects : MonoBehaviour {
    private Target _target;

    [Header("Numbers")]
    public GameObject point1;
    public GameObject point2;
    public GameObject point3;

    // Called on the first frame this script is created.
    private void Start() {
        _target = GetComponent<Target>();
        _target.Destroyed += Target_OnDestroyed;
    }

    // Called when a target is destroyed.
    protected virtual void Target_OnDestroyed(object sender, EventArgs e) {
        _target.Destroyed -= Target_OnDestroyed;
        if (_target.Points == 1) {
            InstantiateHelper(point1);
        } else if (_target.Points == 2) {
            InstantiateHelper(point2);
        } else if (_target.Points == 3) {
            InstantiateHelper(point3);
        }
    }

    // Instantiates a prefab. If possible, makes the prefab face the camera.
    private void InstantiateHelper(GameObject prefab) {
        var effect = Instantiate(prefab, transform.position, Quaternion.identity);
        if (Focus.Instance != null) {
            effect.transform.LookAt(Focus.Instance.transform);
        }
    }
}
