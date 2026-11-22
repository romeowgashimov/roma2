using Godot;

namespace ROMA2.Client.Logic.Interfaces;

public interface IClientCharacter
{
    AnimationTree AnimationTree { get; }
    AnimationNodeStateMachinePlayback AnimationStateMachine { get; set; }
}