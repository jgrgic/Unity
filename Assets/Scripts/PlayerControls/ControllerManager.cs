using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

// Manages the VR controllers that the user is using.
public class ControllerManager : MonoBehaviour {
    public const SteamVR_Input_Sources LEFT_INPUT_SOURCE = SteamVR_Input_Sources.LeftHand;
    public const SteamVR_Input_Sources RIGHT_INPUT_SOURCE = SteamVR_Input_Sources.RightHand;

    [Header("Controller Actions")]
    public SteamVR_Action_Single Squeeze;
    public SteamVR_Action_Boolean Trigger;
    public SteamVR_Action_Vibration HapticFeedback;

    [Header("Controllers")]
    public GameObject _leftController;
    public GameObject _rightController;

    [Header("Settings")]
    [Range(0f, 1f)] [Tooltip("Determines how hard the player has to press down on the squeeze button before it is considered as squeezed.")]
    [SerializeField] private float _squeezingThreshold = 0.8f;
    [SerializeField] private bool _usingHaptics = false;

    // Variables that keep track of the state of the controllers.
    private ControllerState _leftControllerState;
    private ControllerState _rightControllerState;

    // The singleton instance of this class.
    public static ControllerManager Instance {
        get; private set;
    }

    // The holding threshold for the controllers.
    public float HoldingThreshold {
        get { return Mathf.Clamp(_squeezingThreshold, 0f, 1f); }
        set {
            if (value > 1f || value < 0f) Debug.LogErrorFormat("Error - 'Holding Threshold = {0}' is out of bounds", value);
            _squeezingThreshold = value;
        }
    }

    // Determines if this controller manager is using haptics.
    public bool UsingHaptics {
        get { return _usingHaptics; }
        set {
            Debug.LogWarningFormat("Warning - 'Using Haptics = {0}' changed in Run Time", value);
            _usingHaptics = value;
        }
    }

    // First function to be called.
    private void Awake() {
        CreateSingleton(this);
        OnSqueeze(Squeeze, LEFT_INPUT_SOURCE,  Squeeze.GetAxis(LEFT_INPUT_SOURCE),  0);
        OnSqueeze(Squeeze, RIGHT_INPUT_SOURCE, Squeeze.GetAxis(RIGHT_INPUT_SOURCE), 0);
    }

    // Called on the first frame this script is created.
    private void Start() {
        GetControllerScript<ControllerPickup>(LEFT_INPUT_SOURCE ).StartedHolding += (sender, e) => ControllerPickup_OnStartedHolding(LEFT_INPUT_SOURCE , sender, e);
        GetControllerScript<ControllerPickup>(RIGHT_INPUT_SOURCE).StartedHolding += (sender, e) => ControllerPickup_OnStartedHolding(RIGHT_INPUT_SOURCE, sender, e);

        GetControllerScript<ControllerPickup>(LEFT_INPUT_SOURCE ).StoppedHolding += (sender, e) => ControllerPickup_OnStoppedHolding(LEFT_INPUT_SOURCE , sender, e);
        GetControllerScript<ControllerPickup>(RIGHT_INPUT_SOURCE).StoppedHolding += (sender, e) => ControllerPickup_OnStoppedHolding(RIGHT_INPUT_SOURCE, sender, e);
    }

    // Called when this script is destroyed.
    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    // Maps the controller actions to the keyboard.
    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            OnSqueeze(Squeeze, LEFT_INPUT_SOURCE, 0.9f, 0);
        } else if (Input.GetKeyUp(KeyCode.S)) {
            OnSqueeze(Squeeze, LEFT_INPUT_SOURCE, 0.1f, 0);
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            OnTriggerDown(Trigger, LEFT_INPUT_SOURCE);
        } else if (Input.GetKeyUp(KeyCode.X)) {
            OnTriggerUp(Trigger, LEFT_INPUT_SOURCE);
        }
    }

    // Called when this gameobject is enabled.
    private void OnEnable() {
        Squeeze.AddOnAxisListener(OnSqueeze, LEFT_INPUT_SOURCE);
        Squeeze.AddOnAxisListener(OnSqueeze, RIGHT_INPUT_SOURCE);

        Trigger.AddOnStateDownListener(OnTriggerDown, LEFT_INPUT_SOURCE);
        Trigger.AddOnStateDownListener(OnTriggerDown, RIGHT_INPUT_SOURCE);

        Trigger.AddOnStateUpListener(OnTriggerUp, LEFT_INPUT_SOURCE);
        Trigger.AddOnStateUpListener(OnTriggerUp, RIGHT_INPUT_SOURCE);
    }

    // Called when this gameobject is distabled.
    private void OnDisable() {
        Squeeze.RemoveOnAxisListener(OnSqueeze, LEFT_INPUT_SOURCE);
        Squeeze.RemoveOnAxisListener(OnSqueeze, RIGHT_INPUT_SOURCE);

        Trigger.RemoveOnStateDownListener(OnTriggerDown, LEFT_INPUT_SOURCE);
        Trigger.RemoveOnStateDownListener(OnTriggerDown, RIGHT_INPUT_SOURCE);

        Trigger.RemoveOnStateUpListener(OnTriggerUp, LEFT_INPUT_SOURCE);
        Trigger.RemoveOnStateUpListener(OnTriggerUp, RIGHT_INPUT_SOURCE);
    }

    // Initialize the singleton or destroy copies.
    public static void CreateSingleton(ControllerManager controllerManager) {
        if (Instance == null) {
            Instance = controllerManager;
        } else {
            Destroy(controllerManager);
        }
    }

    // Handles what to do when a controllable objects stops tracking a user's controllers.
    public static void HapticFeedback_Execute(SteamVR_Input_Sources fromSource, float duration, float frequency, float amplitude, float secondsFromNow = 0) {
        if (Instance.UsingHaptics) {
            Instance.HapticFeedback.Execute(secondsFromNow, duration, frequency, amplitude, fromSource);
        }
    }

    // Gets the left or right controller.
    public GameObject GetController(SteamVR_Input_Sources fromSource) {
        if (fromSource == LEFT_INPUT_SOURCE) {
            return _leftController;
        } else {
            return _rightController;
        }
    }

    // Gets the state of the controller.
    public ControllerState GetControllerState(SteamVR_Input_Sources fromSource) {
        if (fromSource == LEFT_INPUT_SOURCE) {
            return _leftControllerState;
        } else {
            return _rightControllerState;
        }
    }

    // Sets the state of the controller.
    public void SetControllerState(SteamVR_Input_Sources fromSource, ControllerState controllerState) {
        if (fromSource == LEFT_INPUT_SOURCE) {
            _leftControllerState = controllerState;
        } else {
            _rightControllerState = controllerState;
        }
    }

    // Gets a script from the left or right controller.
    public T GetControllerScript<T>(SteamVR_Input_Sources fromSource) {
        return GetController(fromSource).GetComponent<T>();
    }

    // Called when the user squeezes the controller.
    protected virtual void OnSqueeze(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta) {
        if (newAxis >= HoldingThreshold) {
            var trueSqueezingState = GetControllerState(fromSource).NewSqueezingState(true);

            // If previously not squeezing and is now squeezing.
            if (!GetControllerState(fromSource).Squeezing) {
                SetControllerState(fromSource, trueSqueezingState);
                OnSqueezeDown(fromAction, fromSource, newAxis, newDelta);
            } else {
                SetControllerState(fromSource, trueSqueezingState);
            }
        } else {
            var falseSqueezingState = GetControllerState(fromSource).NewSqueezingState(false);

            // If previously squeezing and is now not squeezing.
            if (GetControllerState(fromSource).Squeezing) {
                SetControllerState(fromSource, falseSqueezingState);
                OnSqueezeUp(fromAction, fromSource, newAxis, newDelta);
            } else {
                SetControllerState(fromSource, falseSqueezingState);
            }
        }
    }

    // Called the frame the user squeezes down on the controller.
    protected virtual void OnSqueezeDown(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta) {
        RaycastHit[] hits = LaserSphereCastAll(fromSource, 0.2f, 0.0f);
        var query = QueryHits<IHoldable>(hits, h => h != null);

        // If there is an item, pick it up.
        if (query.Count > 0) {
            var holdable = query.First();
            GetControllerScript<ControllerPickup>(fromSource).PickUp(holdable);
        }
    }

    // Called the frame the user releases the controller.
    protected virtual void OnSqueezeUp(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta) {
        // If the user is holding an item, let go of it.
        if (GetControllerState(fromSource).Holding != null) {
            GetControllerScript<ControllerPickup>(fromSource).LetGo();
        }
    }

    // Called when the user starts holding a holdable object.
    protected virtual void ControllerPickup_OnStartedHolding(SteamVR_Input_Sources fromSource, object sender, EventArgs e) {
        var holdable = GetControllerScript<ControllerPickup>(fromSource).Holdable;

        holdable.StartHolding();

        var holdingGameObjectState = GetControllerState(fromSource).NewHoldingState(holdable);
        SetControllerState(fromSource, holdingGameObjectState);
    }

    // Called when the user drops a holdable object.
    protected virtual void ControllerPickup_OnStoppedHolding(SteamVR_Input_Sources fromSource, object sender, EventArgs e) {
        var holdable = GetControllerState(fromSource).Holding;

        holdable.StopHolding();

        var holdingNothingState = GetControllerState(fromSource).NewHoldingState(null);
        SetControllerState(fromSource, holdingNothingState);
    }

    // Called when the trigger on the controller is released.
    protected virtual void OnTriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (GetControllerState(fromSource).Holding != null) {
            var gun = GetControllerState(fromSource).Holding.PhysicsBody.GetComponent<IGun>();
            if (gun != null) {
                gun.TriggerDown();
            }
        } else {
            RaycastHit[] hits = LaserSphereCastAll(fromSource, 0.2f, 0.0f);
            var query = QueryHits<IInteractableState>(hits, g => g != null);

            // If there is an interactable item, start interacting with it.
            if (query.Count > 0) {
                var interaction = query.First();
                StartInteracting(fromSource, interaction);
            }
        }
    }

    // Called when the trigger on the controller is pushed down.
    protected virtual void OnTriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (GetControllerState(fromSource).Holding != null) {
            var gun = GetControllerState(fromSource).Holding.PhysicsBody.GetComponent<IGun>();
            if (gun != null) {
                gun.TriggerUp();
            }
        }

        StopInteracting(fromSource);  // Always stop interacting with interactable objects when releasing the trigger button.
    }

    // Starts interacting with an interactable component.
    public void StartInteracting(SteamVR_Input_Sources fromSource, IInteractableState interaction) {
        if (GetControllerState(fromSource).Interacting != interaction) {
            // If already interacting with an item, then stop interacting with it.
            if (GetControllerState(fromSource).Interacting != null) {
                StopInteracting(fromSource);
            }

            // Start interacting with the new item.
            Transform controller = GetController(fromSource).transform;
            interaction.StartInteraction(controller);
            interaction.Interrupted += Interaction_OnInterrupted;

            var startedInteractingState = GetControllerState(fromSource).NewInteractingState(interaction);
            SetControllerState(fromSource, startedInteractingState);
        }
    }

    // Stops interacting with the interactable component that is currently being interacted with.
    public void StopInteracting(SteamVR_Input_Sources fromSource) {
        var interaction = GetControllerState(fromSource).Interacting;
        if (interaction != null) {
            Transform controller = GetController(fromSource).transform;
            interaction.EndInteraction(controller);
        }
    }

    // Called when an interaction is interrupted.
    protected virtual void Interaction_OnInterrupted(object sender, InteractionInterruptEventArgs e) {
        var interaction = (IInteractableState)sender;
        interaction.Interrupted -= Interaction_OnInterrupted;

        // Determines which controller was interrupted.
        SteamVR_Input_Sources fromSource = MapControllerToSource(e.Controller);
        if (fromSource == SteamVR_Input_Sources.Any) {
            Debug.LogErrorFormat("Error - 'Controller {0} - not mapped to left or right input source", e.Controller);
            return;
        }

        var stoppedInteractingState = GetControllerState(fromSource).NewInteractingState(null);
        SetControllerState(fromSource, stoppedInteractingState);
    }

    // Checks if the sphere sweep intersects any collider.
    private bool LaserSphereCast(SteamVR_Input_Sources fromSource, float radius, out RaycastHit hit, float maxDistance) {
        GameObject controller = GetController(fromSource);
        return Physics.SphereCast(controller.transform.position, radius, controller.transform.forward, out hit, maxDistance);
    }

    // Checks if the sphere sweep intersects any collider.
    private RaycastHit[] LaserSphereCastAll(SteamVR_Input_Sources fromSource, float radius, float maxDistance) {
        GameObject controller = GetController(fromSource);
        return Physics.SphereCastAll(controller.transform.position, radius, controller.transform.forward, maxDistance);
    }

    // Maps a controller to the source input.
    private SteamVR_Input_Sources MapControllerToSource(Transform controller) {
        if (controller == _leftController.transform) {
            return LEFT_INPUT_SOURCE;
        } else if (controller == _rightController.transform) {
            return RIGHT_INPUT_SOURCE;
        } else {
            return SteamVR_Input_Sources.Any;
        }
    }

    // Queries a list of Raycast hits.
    private List<T> QueryHits<T>(RaycastHit[] hits, Func<T, bool> filter) {
        return hits
            .Select(h => h.transform.gameObject.GetComponent<T>())
            .Where(c => filter(c))
            .ToList();
    }
}
