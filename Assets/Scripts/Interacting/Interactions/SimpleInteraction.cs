using System;

// Simple interaction without any functionality used for prototyping

[Serializable]
public class SimpleInteraction : Interaction
{
/*    public override void AcceptInteractionVisitor(IInteractionVisitor visitor)
    {
        visitor.Visit(this);
    }*/

    public override void PerformInteraction(Interactor interactor)
    {
        // No functionality
    }
}
