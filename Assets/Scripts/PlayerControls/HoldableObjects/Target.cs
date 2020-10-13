using System;
using UnityEngine;

// Models a target.
public class Target : MonoBehaviour {
    public event EventHandler Destroyed;

    // Keeps track of the state of this target.
    [SerializeField] [Range(1,3)] private int points = 1;
    private bool isDestroyed = false;

    // Gets the number of points this target is worth.
    public int Points {
        get { return points; }
        set { points = value; }
    }

    // Gets whether this target has been destroyed or not.
    public bool IsDestroyed {
        get { return isDestroyed; }
    }

    // Called on the first frame this script is created.
    private void Start() {
        TargetManager.Instance.AddTarget(this);
    }

    // Hits this target.
    public void Hit() {
        if (isDestroyed == false) {
            isDestroyed = true;
            gameObject.SetActive(false);
            OnDestroyed(EventArgs.Empty);
        }
    }

    // Called when this target has been destroyed.
    protected virtual void OnDestroyed(EventArgs e) {
        var handler = Destroyed;
        if (handler != null) {
            handler(this, e);
        }
    }
}
