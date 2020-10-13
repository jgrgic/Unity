using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows for any one object in a scene to be accessed quickly by any object provided that this script is attached to it.
public class Focus : MonoBehaviour {
    // The singleton instance of this class.
    public static Focus Instance {
        get; private set;
    }

    // First function to be called.
    private void Awake() {
        CreateSingleton(this);
    }

    // Called when this script is destroyed.
    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    // Initialize the singleton or destroy copies.
    public static void CreateSingleton(Focus targetManager) {
        if (Instance == null) {
            Instance = targetManager;
        } else {
            Destroy(targetManager);
        }
    }
}
