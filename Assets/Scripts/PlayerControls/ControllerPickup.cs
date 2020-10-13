using System;
using UnityEngine;

// Manages picking up game objects.
public class ControllerPickup : MonoBehaviour {
    public event EventHandler StartedHolding;
    public event EventHandler StoppedHolding;

    // Inspector values.
    public float _initialBreakForce = 1000;

    // Private cached variables.
    private FixedJoint _fixedJoint;
    private Vector3 _lastPosition;
    private Vector3 _velocity;

    // Gets the current object being held.
    public IHoldable Holdable {
        get {
            if (_fixedJoint != null && _fixedJoint.connectedBody != null) {
                return _fixedJoint.connectedBody.GetComponent<IHoldable>();
            }
            return null;
        }
    }

    // Called on the first frame this script is created.
    private void Start() {
        InitializeFixedJoint();

        _lastPosition = transform.position;
        _velocity = Vector3.zero;
    }

    // Called every frame.
    private void Update() {
        _velocity = transform.position - _lastPosition;
        _lastPosition = transform.position;
    }

    // Called when the fixed joint breaks.
    private void OnJointBreak(float breakForce) {
        InitializeFixedJoint(true);
        OnStoppedHolding(EventArgs.Empty);
    }

    // Initializes the fixed joint.
    public void InitializeFixedJoint(bool addImmediately = false) {
        _fixedJoint = GetComponent<FixedJoint>();
        if (addImmediately || _fixedJoint == null) {
            _fixedJoint = gameObject.AddComponent<FixedJoint>() as FixedJoint;
        }

        _fixedJoint.breakForce = _initialBreakForce;
        _fixedJoint.connectedBody = null;
    }

    // Picks up a game object.
    public void PickUp(IHoldable holdable) {
        // TODO: set the anchor to the hand with:  holdable.rigidbody.transform
        if (_fixedJoint.connectedBody != holdable.PhysicsBody) {
            // If already holding an item, then let go of it.
            if (_fixedJoint.connectedBody != null) {
                LetGo();
            }

            // Hold the new item.
            holdable.PhysicsBody.transform.position = transform.position;
            _fixedJoint.connectedBody = holdable.PhysicsBody;
            OnStartedHolding(EventArgs.Empty);
        }
    }

    // Lets go of the currently held game object.
    public void LetGo() {
        if (_fixedJoint.connectedBody != null) {
            _fixedJoint.connectedBody.GetComponent<Rigidbody>().velocity = _velocity / Time.deltaTime;
            _fixedJoint.connectedBody = null;
            OnStoppedHolding(EventArgs.Empty);
        }
    }

    // Called whenever a object is picked up.
    protected virtual void OnStartedHolding(EventArgs e) {
        var handler = StartedHolding;
        if (handler != null) {
            handler(this, e);
        }
    }

    // Called whenever a object is dropped.
    protected virtual void OnStoppedHolding(EventArgs e) {
        var handler = StoppedHolding;
        if (handler != null) {
            handler(this, e);
        }
    }
}
