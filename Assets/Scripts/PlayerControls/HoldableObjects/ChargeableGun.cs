using System;
using System.Collections.Generic;
using UnityEngine;

// A gun that charges a bullet.
public class ChargeableGun : MonoBehaviour, IHoldable, IGun {
    public event Action<bool> Charged;

    [Header("Prefabs")]
    public Bullet bulletTemplate;

    [Header("Shooting Information")]
    public Transform spawnLocation;
    [SerializeField] private float _minPower;
    [SerializeField] private float _maxPower;
    [Range(0, 10)] [SerializeField] private float _timeSpan = 3;

    // Keeps track of the state of this gun.
    private float _time;
    private float _power;
    private Bullet _bullet = null;

    // Gets the Rigidbody of this game object.
    public Rigidbody PhysicsBody {
        get { return GetComponent<Rigidbody>(); }
    }

    // Updates this script in real time.
    private void Update() {
        if (_bullet != null) {
            _time += Time.deltaTime;
            _time = Mathf.Clamp(_time, 0, 100);  // Clamps the value to max sure that there are never any overflow errors.

            // Once time goes over the time span, then power is maximized.
            if (_time >= _timeSpan) {
                _power = _maxPower;
                OnCharged(true);
            }
        }
    }

    // Action for when the gun starts being held.
    public void StartHolding() { }

    // Action for when the gun is released.
    public void StopHolding() {
        OnCharged(false);
    }

    // Action for when the trigger of the gun is pressed down.
    public void TriggerDown() {
        _time = 0;
        _power = _minPower;

        _bullet = Instantiate(bulletTemplate, spawnLocation.position, Quaternion.identity);
        _bullet.GetComponent<Rigidbody>().isKinematic = true;
        _bullet.transform.SetParent(transform);

        var collider = GetComponent<Collider>();
        if (collider != null) {
            _bullet.AddException(collider);
        }
    }

    // Action for when the trigger of the gun is released.
    public void TriggerUp() {
        if (_bullet != null) {
            _bullet.Shoot(transform.forward, _power);
            _bullet.GetComponent<Rigidbody>().isKinematic = false;
            _bullet.transform.SetParent(null);
            _bullet = null;

            OnCharged(false);
        }
    }

    // Sets the power range of this gun.
    public void SetPowerRange(float minPower, float maxPower) {
        _minPower = minPower;
        _maxPower = maxPower;
    }

    // Called when this gun has been charged.
    protected virtual void OnCharged(bool isCharged) {
        var handler = Charged;
        if (handler != null) {
            handler(isCharged);
        }
    }
}
