using System;
using UnityEngine;

// Event arguments for interaction interruptions.
public class InteractionInterruptEventArgs : EventArgs {
    public InteractionInterruptReasons Reason { get; }
    public Transform Controller { get; }

    // Initializes this event argument.
    public InteractionInterruptEventArgs(InteractionInterruptReasons reason) {
        Reason = reason;
        Controller = null;
    }

    // Initializes this event argument.
    public InteractionInterruptEventArgs(InteractionInterruptReasons reason, Transform controller) {
        Reason = reason;
        Controller = controller;
    }

    // Event argument with no data.
    public new static InteractionInterruptEventArgs Empty {
        get { return new InteractionInterruptEventArgs(InteractionInterruptReasons.Unknown); }
    }

    // Event argument for when the user interrupts the interaction.
    public static InteractionInterruptEventArgs InterruptedByUser {
        get { return new InteractionInterruptEventArgs(InteractionInterruptReasons.InterruptedByUser); }
    }

    // Event argument for when the world constraints interrupt the interaction.
    public static InteractionInterruptEventArgs InterruptedByWorld {
        get { return new InteractionInterruptEventArgs(InteractionInterruptReasons.InterruptedByWorld); }
    }

    // Event argument for when the interaction is finished.
    public static InteractionInterruptEventArgs InteractionFinished {
        get { return new InteractionInterruptEventArgs(InteractionInterruptReasons.InteractionFinished); }
    }
}
