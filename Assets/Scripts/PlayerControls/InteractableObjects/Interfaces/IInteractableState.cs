using System;
using UnityEngine;

// Interface for interactable objects that change their state.
public interface IInteractableState {
    event EventHandler<InteractionInterruptEventArgs> Interrupted;
    event EventHandler ChangedStates;
    void StartInteraction(Transform controller);
    void EndInteraction(Transform controller);
}
