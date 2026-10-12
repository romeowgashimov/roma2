using Godot;
using ROMA2.Shared;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _bridge;
    [Export] private Network.Client _client;
    [Export] private StartMenu _startMenu;

    public override void _Ready()
    {
        CharactersConfig charConfig = new(false);
        
        _client.Init();
        _startMenu.Init();
    }

    public override void _Process(double delta)
    {
        
    }
}