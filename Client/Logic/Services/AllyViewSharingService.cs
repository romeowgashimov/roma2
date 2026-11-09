using System;
using Godot;
using ROMA2.Client.Logic.Controllers;
using ROMA2.Client.Logic.Core;
using ROMA2.Client.Logic.Repositories;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Client.Logic.Services;

public class AllyViewSharingService : ITickable, IDisposable
{
    private const float INTERPOLATION_COEFFICIENT = 30f; 

    private NetworkBridge _bridge;
    private CameraPivotControllerRepository _pivotRepository;
    private PlayerRepository _playerRepository;
    private long _myPeer;
    private TeamType _myTeam = TeamType.Unknown;
    private InputHandler _inputHandler;
    private InputSynchronizerRepository _inputRepository;
    private CameraPivotController _allyPivot;
    private Vector3 _allyMouseDirection;
    private InputSynchronizer _allySync;
    private CameraPivotControllerService _pivotService;

    public AllyViewSharingService(
        NetworkBridge bridge,
        CameraPivotControllerRepository pivotRepository,
        long myPeer,
        PlayerRepository playerRepository,
        InputHandler inputHandler,
        InputSynchronizerRepository inputRepository,
        CameraPivotControllerService pivotService)
    {
        _bridge = bridge;
        _pivotRepository = pivotRepository;
        _myPeer = myPeer;

        _playerRepository = playerRepository;
        _inputHandler = inputHandler;
        _inputRepository = inputRepository;

        _pivotService = pivotService;

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
        if (!_pivotRepository.TryGet(receiverPeer, out _allyPivot) 
            || !_pivotRepository.TryGet(_myPeer, out var myPivot)) return;

        var receiverCamera = _allyPivot.Camera;
        var myCamera = myPivot.Camera;

        receiverCamera.Current = state;
        myCamera.Current = !state;

        if (state)
        {
            _allySync = _inputRepository.Get(receiverPeer);
            
            _pivotService.Enabled = false;
        }
        else 
        {
            _allyPivot = null;
            _allySync = null;

            _pivotService.Enabled = true;
        }
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

    public void Tick(float delta)
    {
        if (_allySync == null || _allyPivot == null) return;

        if (_allySync.MouseDirection.Equals(Vector3.Zero)) return;

        _allyMouseDirection = _allyMouseDirection.Lerp(
            -_allySync.MouseDirection, 
            INTERPOLATION_COEFFICIENT * delta);

        _allyPivot.Tick(_allyMouseDirection);
    }
}