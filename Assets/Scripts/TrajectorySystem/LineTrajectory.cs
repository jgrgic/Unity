using System;
using UnityEngine;

// THIS DOES NOT WORKKKKKKKKKKKK!!!!!!!! MUST FIX!!!

// Displays a trajectory line.
[RequireComponent(typeof(LineRenderer))]
public class LineTrajectory : MonoBehaviour {
    private LineRenderer _lineRenderer;

    [Header("Trajectory Properties")]
    [SerializeField] private bool _useForward;
    [SerializeField] private Vector3 _userAssignedDirection;
    [SerializeField] [Min(0)] private float _startSpeed;

    [Header("Line Properties")]
    [SerializeField] [Range(1, 64)] private int _iterations = 1;
    [SerializeField] [Min(0)] private float _distance = 20;

    [Header("Renderer Properties")]
    [SerializeField] private bool _drawOnUpdate = true;

    // If true, uses transform.forward as the starting direction, otherwise, uses the assigned Start Direction.
    public bool UseForward {
        get { return _useForward; }
        set { _useForward = value; }
    }

    // Gets or sets the starting direction of the line.
    public Vector3 UserAssignedDirection {
        get { return _userAssignedDirection; }
        set { _userAssignedDirection = value; }
    }

    // Gets the true starting direction of the line.
    public Vector3 StartDirection {
        get {
            if (_useForward) {
                return Vector3.Normalize(transform.forward);
            } else {
                return Vector3.Normalize(_userAssignedDirection);
            }
        }
    }

    // Gets or sets the starting speed of the trajectory.
    public float StartSpeed {
        get { return _startSpeed; }
        set {
            if (value < 0) {
                Debug.LogErrorFormat("{0} - ValueOutOfBoundsException - trying to set StartSpeed = {1}", GetType(), value);
                return;
            }

            _startSpeed = value;
        }
    }

    // The starting velocity of the trajectory.
    public Vector3 StartVelocity {
        get { return StartDirection * StartSpeed; }
    }

    // Gets or sets the number of iterations for the resolution of the line.
    public int Iterations {
        get { return _iterations; }
        set {
            if (value < 2) {
                Debug.LogErrorFormat("{0} - ValueOutOfBoundsException - trying to set Iterations = {1}", GetType(), value);
                return;
            }

            _iterations = value;
        }
    }

    // If true, then draws the line every update loop.
    public bool DrawOnUpdate {
        get { return _drawOnUpdate; }
        set { _drawOnUpdate = value; }
    }

    // Gets or sets the distance of the line.
    public float Distance {
        get { return _distance; }
        set {
            if (value < 0) {
                Debug.LogErrorFormat("{0} - ValueOutOfBoundsException - trying to set Distance = {1}", GetType(), value);
                return;
            }

            _distance = value;
        }
    }

    // First function to be called.
    private void Awake() {
        _lineRenderer = GetComponent<LineRenderer>();
        DrawTrajectoryLine();
    }

    // Renders a trajectory line every frame.
    private void Update() {
        // Renders the trajectory line.
        if (_drawOnUpdate && _lineRenderer.enabled) {
            DrawTrajectoryLine();
        }
    }

    // Enables or diables the line renderer.
    public void LineRenderer_SetActive(bool value) {
        _lineRenderer.enabled = value;

        // When set to true, immediately redraws the line (otherwise, might get some graphical error).
        if (value) {
            DrawTrajectoryLine();
        }
    }

    // Draws the trajectory line.
    public void DrawTrajectoryLine() {
        // Only change the number of positions if it becomes different.
        if (_lineRenderer.positionCount != _iterations + 1) {
            _lineRenderer.positionCount = _iterations + 1;
        }

        Debug.Log("help");
        _lineRenderer.SetPosition(0, transform.position);
        DrawTrajectoryArc(1, StartVelocity);
    }

    // Draws the trajectory arc.
    public void DrawTrajectoryArc(int index, Vector3 lastVelocity) {
        Debug.Log(lastVelocity);
        if (index < _lineRenderer.positionCount) {
            float segmentLength = Distance / Iterations;

            // Calculates the data for the last point.
            Vector3 lastPosition = _lineRenderer.GetPosition(index - 1);

            // Calculates the data for the next point (this point).
            GetTime(lastVelocity, Physics.gravity, index, out float time1, out float time2);
            float time = Mathf.Max(time1, time2);

            // Use kinematic formulas to calculate data.
            Vector3 nextVelocity = lastVelocity + (Physics.gravity * time);
            float delta_x = lastVelocity.x * time;
            float delta_y = lastVelocity.y * time + (0.5f * Physics.gravity.y * Mathf.Pow(time, 2));
            float delta_z = lastVelocity.z * time;

            // Calculates the data for this position.
            Vector3 nextPosition = lastPosition + new Vector3(delta_x, delta_y, delta_z);
            Vector3 direction = nextPosition - lastPosition;

            // Shoots a raycast to see if there is any collider that blocks the line.
            if (Physics.Raycast(lastPosition, direction, out RaycastHit hit, segmentLength)) {
                _lineRenderer.SetPosition(index, hit.point);
                DrawTrajectoryArcFinish(index + 1);
            } else {
                _lineRenderer.SetPosition(index, nextPosition);
                DrawTrajectoryArc(index + 1, nextVelocity);
            }
        }
    }

    // Draws the trajectory arc.
    public void DrawTrajectoryArc2(int index, Vector3 lastVelocity) {
        if (index < _lineRenderer.positionCount) {
            float segmentLength = Distance / Iterations;

            // Calculates the data for the last point.
            Vector3 lastPosition = _lineRenderer.GetPosition(index - 1);
            Vector3 lastDirection = Vector3.Normalize(lastVelocity);

            // Calculates the data for the next point (this point).
            Vector3 nextVelocity = lastVelocity + (Physics.gravity * GetTimeToReachPoint(index));
            Vector3 nextPosition = lastPosition + (lastDirection * segmentLength);
            Vector3 nextDirection = nextPosition - lastPosition;

            // Shoots a raycast to see if there is any collider that blocks the line.
            if (Physics.Raycast(lastPosition, nextDirection, out RaycastHit hit, segmentLength)) {
                _lineRenderer.SetPosition(index, hit.point);
                DrawTrajectoryArcFinish(index + 1);
            } else {
                _lineRenderer.SetPosition(index, nextPosition);
                DrawTrajectoryArc2(index + 1, nextVelocity);
            }
        }
    }

    // Finishes drawing the arc from the last point.
    public void DrawTrajectoryArcFinish(int index) {
        if (index < _lineRenderer.positionCount) {
            Vector3 lastPosition = _lineRenderer.GetPosition(index - 1);

            _lineRenderer.SetPosition(index, lastPosition);
            DrawTrajectoryArcFinish(index + 1);
        }
    }

    // 
    private void GetTime(Vector3 velocity, Vector3 acceleration, int index, out float time1, out float time2) {
        Vector3 last = _lineRenderer.GetPosition(index - 1);
        float length = Distance / Iterations;
        float l_dist = Vector3.Normalize(velocity * length).y;

        // Calculates the time to reach a point from a position.
        if (acceleration.y != 0) {
            float timeToMaximum = (0 - velocity.y) / acceleration.y;
            float distToMaximum = ((0 + velocity.y) / 2) * timeToMaximum;

            if (acceleration.y > 0) {
                if (velocity.y >= 0) {
                    QuadraticSolve(0.5f * acceleration.y, velocity.y, -l_dist, out time1, out time2);
                } else {
                    if (l_dist <= distToMaximum) {
                        QuadraticSolve(0.5f * acceleration.y, velocity.y, -l_dist, out time1, out time2);
                    } else {
                        //Debug.Log();
                        QuadraticSolve(0.5f * acceleration.y, 0, l_dist - distToMaximum, out float second1, out float second2);
                        time1 = timeToMaximum + second1;
                        time2 = timeToMaximum + second2;
                    }
                }
            } else {
                if (velocity.y <= 0) {
                    QuadraticSolve(0.5f * acceleration.y, velocity.y, -l_dist, out time1, out time2);
                } else {
                    if (l_dist <= distToMaximum) {
                        QuadraticSolve(0.5f * acceleration.y, velocity.y, -l_dist, out time1, out time2);
                    } else {
                        QuadraticSolve(0.5f * acceleration.y, 0, l_dist - distToMaximum, out float second1, out float second2);
                        time1 = timeToMaximum + second1;
                        time2 = timeToMaximum + second2;
                    }
                }
            }
        } else {
            time1 = Mathf.Infinity;
            time2 = Mathf.Infinity;
        }

        //Debug.LogFormat("time1 = {0}, time2 = {1}", time1, time2);
    }

    // Solve quadratic equation.
    private void QuadraticSolve(float a, float b, float c, out float x1, out float x2) {
        Debug.LogFormat("a = {0}, b = {1}, c = {2}", a, b, c);
        x1 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
        x2 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
    }

    // Time to reach an arbitrary point. THIS TOOK SO LONG TO MAKE PLEASE APPREACIATE THIS!!!
    private float GetTimeToReachPoint(int index) {
        float constSpeed = new Vector2(StartVelocity.x, StartVelocity.z).magnitude;
        Vector3 acceleration = Physics.gravity;

        // If the speed of the projected x and z is not equal to zero, then calculate time.
        if (false && constSpeed != 0) {
            float segLength = Distance / Iterations;
            float segDistance = new Vector2(StartDirection.x, StartDirection.z).magnitude * segLength * index;

            return segDistance / constSpeed;
        } else {
            // If velocity and acceleration are going in the same y direction.
            if (StartVelocity.y > 0 == acceleration.y > 0) {
                // If going in the positive y direction, then calculate time.
                if (StartVelocity.y >= 0 && acceleration.y > 0) {
                    float segLength = Distance / Iterations;
                    float segDistance = (transform.position.y + segLength * index) - transform.position.y;

                    float a = 0.5f * acceleration.y;
                    float b = StartVelocity.y;
                    float c = -segDistance;

                    float time1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
                    float time2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;

                    // Returns the positive time.
                    if (time1 > 0) {
                        return time1;
                    } else {
                        return time2;
                    }
                } else if (StartVelocity.y <= 0 && acceleration.y < 0) {
                    float segLength = Distance / Iterations;
                    float segDistance = (transform.position.y - segLength * index) - transform.position.y;

                    float a = 0.5f * acceleration.y;
                    float b = StartVelocity.y;
                    float c = -segDistance;

                    float value1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
                    float value2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;

                    // Returns the positive time.
                    if (value1 > 0) {
                        return value1;
                    } else {
                        return value2;
                    }
                } else {
                    Debug.LogError("GetTimeToReachPoint - trajectile not moving");
                    return Mathf.Infinity;
                }
            } else {
                // Must split calculations into two parts. 
                float timeToMaximum = (0 - StartVelocity.y) / acceleration.y;
                float distToMaximum = ((0 + StartVelocity.y) / 2) * timeToMaximum;

                // If initially going up.
                if (StartVelocity.y > 0) {
                    float segLength = Distance / Iterations;
                    float segDistance = (transform.position.y + segLength * index) - transform.position.y;

                    // If segment ends before reaching the peak.
                    if (segDistance <= distToMaximum) {
                        float a = 0.5f * acceleration.y;
                        float b = StartVelocity.y;
                        float c = -segDistance;

                        float time1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
                        float time2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;

                        // Returns the positive time.
                        if (time1 > 0) {
                            return time1;
                        } else {
                            return time2;
                        }
                    } else {
                        float a = 0.5f * acceleration.y;
                        float b = 0;
                        float c = segDistance - distToMaximum;

                        float time1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
                        float time2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;

                        // Returns the positive time.
                        if (time1 > 0) {
                            return timeToMaximum + time1;
                        } else {
                            return timeToMaximum + time2;
                        }
                    }
                } else {
                    float segLength = Distance / Iterations;
                    float segDistance = (transform.position.y - segLength * index) - transform.position.y;

                    // If segment ends before reaching the peak.
                    if (Mathf.Abs(segDistance) <= Mathf.Abs(distToMaximum)) {
                        float a = 0.5f * acceleration.y;
                        float b = StartVelocity.y;
                        float c = -segDistance;

                        float time1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
                        float time2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;

                        // Returns the positive time.
                        if (time1 > 0) {
                            return time1;
                        } else {
                            return time2;
                        }
                    } else {
                        float a = 0.5f * acceleration.y;
                        float b = 0;
                        float c = segDistance - distToMaximum;

                        float time1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;
                        float time2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4 * a * c)) / 2 * a;

                        // Returns the positive time.
                        if (time1 > 0) {
                            return timeToMaximum + time1;
                        } else {
                            return timeToMaximum + time2;
                        }
                    }
                }
            }
        }
    }

    // Calculates the time to reach the maximum height of the trajectory.
    private float GetTimeToReachMaximum() {
        float initial_velocity_y = StartVelocity.y;
        float final_velocity_y = 0;

        float acceleration_y = Physics.gravity.y;
        return (final_velocity_y - initial_velocity_y) / acceleration_y;  // Calculates the time.
    }
}
