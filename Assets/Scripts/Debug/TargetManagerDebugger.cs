using UnityEngine;

// Debugs the target manager instances.
public class TargetManagerDebugger : MonoBehaviour {
    public KeyCode CountHits = KeyCode.Alpha1;
    public KeyCode AllHits = KeyCode.Alpha2;
    public KeyCode Points = KeyCode.Alpha3;

    // Checks for the user's keyboard inputs.
    private void Update() {
        // Gets the number of hits.
        if (Input.GetKeyDown(CountHits)) {
            Debug.LogFormat("The number of targets hit: {0}", TargetManager.Instance.NumberTargetsHit());
        }


        // Gets if all targets have been hit.
        else if (Input.GetKeyDown(AllHits)) {
            Debug.LogFormat("The player hit all objects: {0}", TargetManager.Instance.PlayerHitAllTargets());
        }


        // Gets number of points.
        else if (Input.GetKeyDown(Points)) {
            Debug.LogFormat("The total points score: {0}", TargetManager.Instance.Points);
        }
    }
}
