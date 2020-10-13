using System;
using UnityEngine;

// Abstract class defining an interaction.
public abstract class AbstractInteractionComponent : MonoBehaviour, IInteractableState {
    public event EventHandler<InteractionInterruptEventArgs> Interrupted;
    public event EventHandler ChangedStates;

    // The current controller.
    public Transform CurrentController {
        get; private set;
    }

    // Keeps track of the location of the controllers.
    public virtual void Update() {
        if (CurrentController != null) {
            Track(CurrentController.position, CurrentController.rotation);
        }
    }

    // Starts interacting with a controller.
    public void StartInteraction(Transform controller) {
        if (CurrentController != controller) {
            // If already being controlled by a different controller, then stop interacting with the first controller.
            if (CurrentController != null) {
                EndInteraction(CurrentController);
            }

            // Starts interacting with the new controller.
            CurrentController = controller;
            OnStartInteraction();
        }
    }

    // Stops interacting with a controller.
    public void EndInteraction(Transform controller) {
        if (CurrentController == controller) {
            OnEndInteraction();
            CurrentController = null;
            OnInterrupted(new InteractionInterruptEventArgs(InteractionInterruptReasons.InterruptedByUser, controller));
        }
    }

    protected abstract void Track(Vector3 location, Quaternion rotation);
    protected abstract void OnStartInteraction();
    protected abstract void OnEndInteraction();

    // Called whenever this monobehaviour changes states.
    protected virtual void OnChangedStates(InteractionInterruptEventArgs e) {
        var handler = ChangedStates;
        if (handler != null) {
            handler(this, e);
        }
    }

    // Called whenever the interaction is interupted.
    protected virtual void OnInterrupted(InteractionInterruptEventArgs e) {
        var handler = Interrupted;
        if (handler != null) {
            handler(this, e);
        }
    }
}
