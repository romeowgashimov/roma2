using Godot;
using ROMA2.Shared;

namespace ROMA2.Client.Logic.Core;

public partial class EntryPoint : Node
{
    [Export] private NetworkBridge _networkBridge;
    [Export] private Network.Client _client;

    public override void _Ready()
    {
        _client.Init();
    }
}