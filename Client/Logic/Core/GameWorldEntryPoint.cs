using Godot;
using ROMA2.Shared;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public partial class GameWorldEntryPoint : Node
{
    [Export] private CharacterSelectionView _charSelectView;
    [Export] private MultiplayerSpawner _spawner;
    
    private NetworkBridge _bridge;


    public void Init(NetworkBridge bridge)
    {
        CharactersConfiguration charConfig = new(false);
        PlayerRepository playerRepos = new();

        _bridge = bridge;

        CharacterSelectionService charSelectService = new(_bridge, playerRepos);
        CharacterSelectionViewModel charSelectViewModel = new(charSelectService);
        _charSelectView.Init(charSelectViewModel);

        CharacterFactory charFactory = new(charConfig, _spawner);

        _bridge.CharacterSelectionApproved += UnSubscribe;
        void UnSubscribe(long _, int __)
        {
            _charSelectView.QueueFree();
            _bridge.CharacterSelectionApproved -= UnSubscribe;
        }
    }
}