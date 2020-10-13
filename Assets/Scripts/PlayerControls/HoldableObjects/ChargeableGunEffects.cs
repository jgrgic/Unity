using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChargeableGun))]
public class ChargeableGunEffects : MonoBehaviour {
    private ChargeableGun _gun;
    [SerializeField] private GameObject _effects;

    // Called on the first frame this script is active.
    private void Start() {
        _gun = GetComponent<ChargeableGun>();
        _gun.Charged += Gun_OnCharged;

        EffectsActive(false);
    }

    // Sets the effects to being active or not active.
    public void EffectsActive(bool value) {
        if (_effects != null) {
            _effects.SetActive(value);
        }
    }

    // Called whenever the gun becomes charged or when it stops being charged.
    protected virtual void Gun_OnCharged(bool isCharged) {
        EffectsActive(isCharged);
    }
}
