using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Explains why an interruption happened.
public enum InteractionInterruptReasons {
    Unknown,                 // Reason is unknown.
    InterruptedByUser,       // User stopped interaction.
    InterruptedByWorld,      // Other reason for interuption (ex. user too far).
    InteractionFinished,     // Interaction ended.
}
