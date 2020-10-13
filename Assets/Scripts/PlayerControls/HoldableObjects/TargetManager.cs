using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Manages all the targets in a scene.
public class TargetManager : MonoBehaviour {
    private int _points = 0;
    private HashSet<Target> _targets;

    // The singleton instance of this class.
    public static TargetManager Instance {
        get; private set;
    }

    // Gets the number of points that the user has gotten from destroying targets.
    public int Points {
        get { return _points; }
    }

    // First function to be called.
    private void Awake() {
        CreateSingleton(this);
        _targets = new HashSet<Target>();
    }

    // Called when this script is destroyed.
    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    // Starts tracking a target.
    public bool AddTarget(Target target) {
        target.Destroyed += Target_OnDestroyed;
        return _targets.Add(target);
    }

    // Returns true if all targets have been hit.
    public bool PlayerHitAllTargets() {
        return _targets.All(target => target.IsDestroyed);
    }

    // Counts the number of targets that have been hit.
    public int NumberTargetsHit() {
        return _targets.Count(target => target.IsDestroyed);
    }

    // Initialize the singleton or destroy copies.
    public static void CreateSingleton(TargetManager targetManager) {
        if (Instance == null) {
            Instance = targetManager;
        } else {
            Destroy(targetManager);
        }
    }

    // Called when a target has been destroyed.
    protected virtual void Target_OnDestroyed(object sender, EventArgs e) {
        var target = (Target)sender;

        _points += target.Points;
        target.Destroyed -= Target_OnDestroyed;
    }
}
