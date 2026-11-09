using Godot;
using ROMA2.Client.Logic.Controllers;

namespace ROMA2.Client.Logic.Interfaces;

public interface IClientPlayableCharacter : IClientCharacter
{
    Camera3D Camera { get; }
    CameraPivotController PivotController { get; }
}