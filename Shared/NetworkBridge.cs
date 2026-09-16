using System;
using Godot;

namespace ROMA2.Shared;

public partial class NetworkBridge : Node
{
    public event Action<long> RequestJoin;
    public event Action ApproveJoin;

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
}