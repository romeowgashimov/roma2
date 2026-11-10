using System;
using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using static ROMA2.Shared.Configurations.ProjectilesConfiguration;

namespace ROMA2.Shared.Projectiles;

public abstract partial class Projectile : Area3D, ITickable, INetworkTransform
{
    protected ProjectileExtendedFactory _projFactory;

    private Vector3 _networkPreviousPosition;
    [Export] public MultiplayerSynchronizer Sync;

    public int Identifier { get; init; }
    public int Reference { get; set; }
    public Character CharacterOwner { get; set; }

    [Export] public Vector3 NetworkPosition { get; set; }
    [Export] public Quaternion NetworkQuaternion { get; set; }
    public List<NetworkState> StateBuffer { get; set; }
    public float VirtualRenderTick { get; set; } = -1;
    private uint _serverTick;
    [Export] public uint ServerTick
    {
        get
        {
            return _serverTick;
        }
        set
        {
            _serverTick = value;

            Synchronized?.Invoke(value);
        }
    }

    public event Action<float> Synchronized;

    protected Projectile()
    {
        Identifier = GetProjectileId(this);
    }
    
    public virtual void Init(ProjectileExtendedFactory projFactory)
    {
        _projFactory = projFactory;
        StateBuffer = new(10);
    }

    public override void _EnterTree()
    {
        Sync.SetMultiplayerAuthority(1);
    }

    public virtual void Start(Character owner) { }
    
    public abstract void Tick(float delta);

    public virtual void Clear()
    {
        VirtualRenderTick = -1;
        StateBuffer.Clear();
    }
}