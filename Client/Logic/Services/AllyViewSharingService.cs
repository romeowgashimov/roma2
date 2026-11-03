using System;
using Friflo.Engine.ECS;
using Godot;
using ROMA2.Client.Logic.Core;
using ROMA2.Client.Logic.Data;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Services;

public class AllyViewSharingService : IDisposable
{
    private NetworkBridge _bridge;
    private CameraPivotControllerRepositoty _pivotRepository;
    private PlayerRepository _playerRepository;
    private long _myPeer;
    private TeamType _myTeam = TeamType.Unknown;
    private EntityStore _store;
    private Entity _cameraEntity;
    private CommandBuffer _cb;
    private InputHandler _inputHandler;
    private InputSynchronizerRepository _inputRepository;

    public AllyViewSharingService(
        NetworkBridge bridge,
        CameraPivotControllerRepositoty pivotRepository,
        long myPeer,
        EntityStore store,
        CommandBuffer cb, 
        PlayerRepository playerRepository,
        InputHandler inputHandler,
        InputSynchronizerRepository inputRepository)
    {
        _bridge = bridge;
        _pivotRepository = pivotRepository;
        _myPeer = myPeer;
        _store = store;
        _cb = cb;
        _playerRepository = playerRepository;
        _inputHandler = inputHandler;
        _inputRepository = inputRepository;

        _bridge.ViewSharingAccess += OnViewSharingAccess;
        _inputHandler.HandleInput += ViewSharingRequest;
    }

    public void OnViewSharingAccess(long receiverPeer, long senderPeer, bool state)
    {
        if (_playerRepository.GetTeam(senderPeer) != _playerRepository.GetTeam(receiverPeer)) 
            return;

        if (receiverPeer == _myPeer) ShareView(senderPeer, state);

        if (senderPeer == _myPeer && state) GetView(receiverPeer, true);
    }

    public void ShareView(long senderPeer, bool state)
    {
        _inputRepository
            .GetSynchronizer(_myPeer)
            .SetVisibilityFor((int)senderPeer, state);

        return;
    }

    public void GetView(long receiverPeer, bool state)
    {
        var receiverCamera = _pivotRepository.Get(receiverPeer).Camera;
        var myCamera = _pivotRepository.Get(_myPeer).Camera;

        receiverCamera.Current = state;
        myCamera.Current = !state;

        if (state) 
        {
            _cameraEntity = _store.CreateEntity(
                new CameraPivotControllerReference { OwnerPeer = receiverPeer });

            _cameraEntity.AddTag<NetworkPlayerTag>();
        }
        else _cb.DeleteEntity(_cameraEntity.Id);
    }

    public void ViewSharingRequest(InputEvent @event)
    {
        if (@event.IsActionPressed("share_ally1") && !@event.IsEcho())
        {
            if (_myTeam == TeamType.Unknown) _myTeam = _playerRepository.GetTeam(_myPeer);

            long peer = _playerRepository.GetPlayer(1, _myTeam);

            if (peer == _myPeer) return;

            _bridge.Rpc(nameof(_bridge.OnViewSharingRequest), peer, true);
        }

        if (@event.IsActionReleased("share_ally1") && !@event.IsEcho())
        {
            long peer = _playerRepository.GetPlayer(1, _myTeam);
            
            if (peer == _myPeer) return;

            GetView(peer, false);

            _bridge.Rpc(nameof(_bridge.OnViewSharingRequest), peer, false);
        }

        if (@event.IsActionPressed("share_ally2") && !@event.IsEcho())
        {
            if (_myTeam == TeamType.Unknown) _myTeam = _playerRepository.GetTeam(_myPeer);

            long peer = _playerRepository.GetPlayer(0, _myTeam);

            if (peer == _myPeer) return;

            _bridge.Rpc(nameof(_bridge.OnViewSharingRequest), peer, true);
        }

        if (@event.IsActionReleased("share_ally2") && !@event.IsEcho())
        {
            long peer = _playerRepository.GetPlayer(0, _myTeam);
            
            if (peer == _myPeer) return;

            GetView(peer, false);

            _bridge.Rpc(nameof(_bridge.OnViewSharingRequest), peer, false);
        }
    }

    public void Dispose()
    {
        _bridge.ViewSharingAccess -= OnViewSharingAccess;
        _inputHandler.HandleInput -= ViewSharingRequest;

        GC.SuppressFinalize(this);
    }
}