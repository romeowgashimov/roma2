using Godot;
using ROMA2.Shared;

namespace ROMA2.Client.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _bridge;
    [Export] private Network.Client _client;
    [Export] private StartMenu _startMenu;

    public override void _Ready()
    {
        _client.Init();
        _startMenu.Init();
    }

    public override void _Process(double delta)
    {
        
    }
}