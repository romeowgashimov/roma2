using System;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Server.Logic.Services;

public class AllyViewSharingService : IDisposable
{
    private NetworkBridge _bridge;
    private PlayerRepository _playerRepository;
    private InputSynchronizerRepository _syncsRepository;

    public AllyViewSharingService(
        NetworkBridge bridge, 
        PlayerRepository playerRepository,
        InputSynchronizerRepository syncsRepository)
    {
        _bridge = bridge;
        _playerRepository = playerRepository;
        _syncsRepository = syncsRepository;

        _bridge.ViewSharingRequest += ValidateSharingRequest;
    }

    public void ValidateSharingRequest(long senderPeer, long receiverPeer, bool state)
    {
        if (_playerRepository.GetTeam(senderPeer) != _playerRepository.GetTeam(receiverPeer)) 
            return;

        _syncsRepository
            .GetSynchronizer(receiverPeer)
            .SetVisibilityFor((int)senderPeer, state);

        _bridge.RpcId(receiverPeer, nameof(_bridge.OnViewSharingAccess), receiverPeer, senderPeer, state);
        _bridge.RpcId(senderPeer, nameof(_bridge.OnViewSharingAccess), receiverPeer, senderPeer, state);
    }

    public void Dispose()
    {
        _bridge.ViewSharingRequest -= ValidateSharingRequest;

        GC.SuppressFinalize(this);
    }
}