using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float speed = 3f;
    public Transform pathParent;
    Transform targetNode;
    int index;

    void OnDrawGizmos() {
        Vector3 from;
        Vector3 to;

        for (int i = 0; i < pathParent.childCount; i++) {
            from = pathParent.GetChild(i).position;
            to = pathParent.GetChild((i+1) % pathParent.childCount).position;
            Gizmos.color = new Color(0, 1, 0);
            if (i+1 == pathParent.childCount) {
                break;
            }
            else {
                Gizmos.DrawLine(from, to);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        targetNode = pathParent.GetChild(index);    
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetNode.position) < 0.1f) {
            if (NodeReached()) {
                Debug.Log("here");
            }
            index++;
            index %= pathParent.childCount;
            targetNode = pathParent.GetChild(index);
        }
    }

    bool NodeReached() {
        return true;
    }
}
