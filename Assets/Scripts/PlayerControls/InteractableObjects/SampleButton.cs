using System;
using UnityEngine;

// Sample script for a button.
public class SampleButton : AbstractInteractionComponent {
    Vector3 lastLocation;

    // Tracks the location of the controllers.
    protected override void Track(Vector3 location, Quaternion rotation) {
        if (location != lastLocation) {
            lastLocation = CurrentController.transform.position;
            Debug.Log("lastLocation: " + lastLocation);
        }
    }

    // Called when the button starts interacting with controller.
    protected override void OnStartInteraction() {
        Debug.Log("SampleButton - OnStartInteraction");
        lastLocation = CurrentController.transform.position;
    }

    // Called when
    protected override void OnEndInteraction() {
        Debug.Log("SampleButton - OnEndInteraction");
    }
}
