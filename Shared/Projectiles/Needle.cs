using Godot;

namespace ROMA2.Shared.Projectiles;

public abstract partial class Needle : Projectile
{
    public Tuman Tuman { get; set; }
    public Vector3 Offset { get; set; }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public virtual void AddChild(int tumanReference)
    {
        var tuman = (Tuman)_projFactory.GetActiveProjectile(tumanReference);

        Tuman = tuman;

        Reparent(Tuman);
    }

    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public virtual void Reset()
    {
        Reparent(Tuman.GetParent());
    }
}