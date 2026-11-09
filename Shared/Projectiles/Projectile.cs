using Godot;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Interfaces;


namespace ROMA2.Shared.Projectiles;

public abstract partial class Projectile : RigidBody3D, ITickable, INetworkTransform
{
    protected ProjectileExtendedFactory _projFactory;

    [Export] public Vector3 NetworkPosition { get; set; }
    [Export] public Vector3 NetworkRotation { get; set; }

    [Export] public MultiplayerSynchronizer Sync;

    public int Identifier { get; init; }
    public int Reference { get; set; }
    

    protected Projectile()
    {
        Identifier = ProjectilesConfiguration.GetProjectileId(this);
    }
    
    public virtual void Init(ProjectileExtendedFactory projFactory)
    {
        _projFactory = projFactory;
    }

    public override void _EnterTree()
    {
        Sync.SetMultiplayerAuthority(1);
    }

    public abstract void Tick(float delta);

    public abstract void Clear();
}