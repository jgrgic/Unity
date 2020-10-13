using UnityEngine;

// Sample script for a gun.
public class SampleGun : MonoBehaviour, IHoldable, IGun {
    // Gets the Rigidbody of this game object.
    public Rigidbody PhysicsBody {
        get { return GetComponent<Rigidbody>(); }
    }

    // Action for when the gun starts being held.
    public void StartHolding() {
        Debug.Log("Sample Gun - Start Holding");
    }

    // Action for when the gun is released.
    public void StopHolding() {
        Debug.Log("Sample Gun - Stop Holding");
    }

    // Action for when the trigger of the gun is pressed down.
    public void TriggerDown() {
        Debug.Log("Sample Gun - Trigger Down");
    }

    // Action for when the trigger of the gun is released.
    public void TriggerUp() {
        Debug.Log("Sample Gun - Trigger Up");
    }
}
