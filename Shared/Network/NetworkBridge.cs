using System;
using Godot;
using Godot.Collections;
using ROMA2.Shared.Projectiles;
namespace ROMA2.Shared.Network;

public sealed partial class NetworkBridge : Node
{
    public event Action<long> RequestJoin;
    public event Action ApproveJoin;
    public event Action<Dictionary> AllPlayersConnected;
    public event Action<long> SelectCharacter;
    public event Action<long, int> CharacterSelected;
    public event Action<long, bool> CharacterSelectionCanceled;
    public event Action<long, int> CharacterSelectionApproved;
    public event Func<long, InputSynchronizer> InputSyncCreation;
    public event Action<long> InputSyncCreationSuccess;
    public event Func<byte[], Projectile> ProjectileCreated;
    public event Action<int> ProjectileDeleted;
    public event Action<long, long, bool> ViewSharingRequest;
    public event Action<long, long, bool> ViewSharingAccess;

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
        {
            RequestJoin = null;
            ApproveJoin = null;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnRequestingJoin()
    {
        long senderId = Multiplayer.GetRemoteSenderId();
        RequestJoin?.Invoke(senderId);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnApprovedJoin()
    {
        ApproveJoin?.Invoke();
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnAllPlayersConnected(Dictionary data)
    {
        AllPlayersConnected?.Invoke(data);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnSelectingCharacter(long selectorPeer)
    {
        SelectCharacter?.Invoke(selectorPeer);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnSelectedCharacter(int characterId)
    {
        long senderId = Multiplayer.GetRemoteSenderId();
        CharacterSelected?.Invoke(senderId, characterId);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnCharacterSelectionCanceled(long selectorPeer, bool characterTaken)
    {
        CharacterSelectionCanceled?.Invoke(selectorPeer, characterTaken);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnCharacterSelectionApproved(long selectorPeer, int characterId)
    {
        CharacterSelectionApproved?.Invoke(selectorPeer, characterId);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnInputSynchronizerCreation(long ownerPeer)
    { 
        InputSyncCreation?.Invoke(ownerPeer);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnInputSynchronizerCreationSuccess()
    {
        long senderId = Multiplayer.GetRemoteSenderId();
        InputSyncCreationSuccess?.Invoke(senderId);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnProjectileCreated(byte[] bytes)
    {
        ProjectileCreated?.Invoke(bytes);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnProjectileDeleted(int localId)
    {
        ProjectileDeleted?.Invoke(localId);
    }

    // state - хотим получить/true, больше не хотим получать/false
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnViewSharingRequest(long receiverPeer, bool state)
    {
        long senderId = Multiplayer.GetRemoteSenderId();
        ViewSharingRequest?.Invoke(senderId, receiverPeer, state);
    }   

    // state - хотим получить/true, больше не хотим получать/false
    [Rpc(MultiplayerApi.RpcMode.Authority, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void OnViewSharingAccess(long receiverPeer, long senderPeer, bool state)
    {
        ViewSharingAccess?.Invoke(receiverPeer, senderPeer, state);
    }
}