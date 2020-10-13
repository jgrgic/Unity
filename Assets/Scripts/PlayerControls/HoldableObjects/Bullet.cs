using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {
    [Header("Effects after collision")]
    public GameObject _afterEffects;

    [Header("Time to live")]
    [Range(0, 15)] [SerializeField] private float _timeLimit;
    private float _time;
    private bool _timerStart = false;

    // A collection of colliders to ignore.
    private HashSet<Collider> _exceptions = new HashSet<Collider>();

    // Gets or sets the time limit for this bullet before it destroys itself.
    public float TimeLimit {
        get { return _timeLimit; }
        set { _timeLimit = value; }
    }

    // Gets or sets if the timer started or not.
    public bool TimerStart {
        get { return _timerStart; }
        set { _timerStart = value; }
    }

    // Called every frame.
    private void Update() {
        if (_timerStart) {
            _time += Time.deltaTime;
            if (_time >= _timeLimit) {
                Destroy(gameObject);
            }
        }
    }

    // Adds a collider to ignore.
    public void AddException(Collider other) {
        if (!_exceptions.Contains(other)) {
            _exceptions.Add(other);
        }
    }

    // Shoots the bullet in the given direction and with the given power.
    public void Shoot(Vector3 direction, float power) {
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.velocity = direction.normalized * power;
        _timerStart = true;
    }

    // Explodes.
    public void Explode() {
        if (_afterEffects != null) {
            Instantiate(_afterEffects, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    // Called when this bullet hits a target.
    protected virtual void OnTriggerEnter(Collider other) {
        var target = other.GetComponent<Target>();
        if (target != null) {
            target.Hit();
            Explode();
        } else if (!HitException(other)) {
            Explode();
        }
    }

    // Determines if an exception was hit by this bullet.
    private bool HitException(Collider other) {
        return _exceptions.Contains(other);
    }
}
