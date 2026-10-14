using Godot;
using ROMA2.Shared;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public partial class GameWorldEntryPoint : Node
{
    [Export] private CharacterSelectionMenu _charSelectMenu;
    [Export] private MultiplayerSpawner _spawner;
    private NetworkBridge _bridge;
    private CharacterSelectionService _charSelectService;
    private CharacterFactory _charFactory;


    public void Init(NetworkBridge bridge, MultiplayerSpawner spawner)
    {
        CharactersConfig charConfig = new(false);

        _bridge = bridge;
        _spawner = spawner;

        CharacterSelectionService charSelectService = new(_bridge);
        _charSelectService = charSelectService;

        _charFactory = new(charConfig, _spawner, _spawner.GetParent());
        GD.Print("afafafwq" + _spawner.GetParent().Name);
        
        _charSelectMenu.Init(_charSelectService);

        _bridge.CharacterSelectionApproved += UnSubscribe;
        void UnSubscribe(long _, int __)
        {
            _charSelectMenu.QueueFree();
            _bridge.CharacterSelectionApproved -= UnSubscribe;
        }
    }
}