using UnityEngine;

// Wrapper for the state of a controller at a point in time.
public readonly struct ControllerState {
    public bool Squeezing { get; }
    public IHoldable Holding { get; }
    public IInteractableState Interacting { get; }

    // Defines an immutable instance of this struct.
    public ControllerState(bool squeezing, IHoldable holding, IInteractableState interacting) {
        Squeezing = squeezing;
        Holding = holding;
        Interacting = interacting;
    }

    // Creates a new state with the given information.
    public ControllerState NewSqueezingState(bool newSqueezing) {
        return new ControllerState(newSqueezing, Holding, Interacting);
    }

    // Creates a new state with the given information.
    public ControllerState NewHoldingState(IHoldable newHolding) {
        return new ControllerState(Squeezing, newHolding, Interacting);
    }

    // Creates a new state with the given information.
    public ControllerState NewInteractingState(IInteractableState newInteracting) {
        return new ControllerState(Squeezing, Holding, newInteracting);
    }
}
