using UnityEngine;

// Interface for holdable game objects.
public interface IHoldable {
    Rigidbody PhysicsBody { get; }
    void StartHolding();
    void StopHolding();
}
