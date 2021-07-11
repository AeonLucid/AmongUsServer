using System;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;

namespace Impostor.Api.Events.System
{
    public interface IHudOverrideSystemEvent : ISystemEvent, IActivatable
    {
        // Does not include anything in particular because it does not need any specialisation : either is it Active, or it is not.
    }
}
