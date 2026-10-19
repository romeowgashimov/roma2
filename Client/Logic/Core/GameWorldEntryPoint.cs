using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Shared;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public partial class GameWorldEntryPoint : Node
{
    [Export] private CharacterSelectionView _charSelectView;
    [Export] private MultiplayerSpawner _charSpawner;
    [Export] private Node _inputRoot;
    
    private NetworkBridge _bridge;
    private EntityStore _store;
    private SystemGroup _group;

    public void Init(NetworkBridge bridge, Network.Client client, EntityStore store, SystemGroup group)
    {
        _group = group;
        _store = store;

        CharactersConfiguration charConfig = new(false);
        long myPeer = client.Multiplayer.GetUniqueId();
        PlayerRepository playerRepos = new() { MyPeer = myPeer };

        _store.CreateEntity(new UniqueEntity("MyPeer"), new PlayerPeer { Value = myPeer });

        _group.Add(new CameraPivotControllerSystem());

        _bridge = bridge;

        CharacterSelectionService charSelectService = new(_bridge, playerRepos);
        CharacterSelectionViewModel charSelectViewModel = new(charSelectService);
        _charSelectView.Init(charSelectViewModel);

        CharacterFactory charFactory = new(charConfig, _charSpawner, _store);
        InputSynchronizerFactory inputSyncFactory = new(_inputRoot, myPeer, false);

        _bridge.InputSyncCreation += inputSyncFactory.Create;

        inputSyncFactory.InputSyncCreated += AddSynchronizer;

        _bridge.CharacterSelectionApproved += UnSubscribe;
        void UnSubscribe(long _, int __)
        {
            _charSelectView.QueueFree();
            _bridge.CharacterSelectionApproved -= UnSubscribe;
        }
    }

    public void AddSynchronizer(InputSynchronizer sync)
    {
        Entity entity = _store.CreateEntity();
        entity.AddComponent(new InputSynchronizerReference { Value = sync });
    }
}