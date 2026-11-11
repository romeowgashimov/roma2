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

    [Export] private float _moveSpeed = 5f;
    public float MoveSpeed 
    {
        get
        {
            return _moveSpeed;
        }
        set
        {
            _moveSpeed = value;

            ServerMaxSpeed = value;
        }
    }

    private Vector3 _networkPreviousPosition;
    [Export] public MultiplayerSynchronizer Synchronizer { get; set; }

    public int Identifier { get; init; }
    public int Reference { get; set; }
    public Character CharacterOwner { get; set; }

    [Export] public Vector3 NetworkPosition { get; set; }
    [Export] public Quaternion NetworkQuaternion { get; set; }
    public List<NetworkState> StateBuffer { get; set; }
    public float VirtualRenderTick { get; set; } = -1;
    [Export] public uint ServerTick { get; set; }
    [Export] public float NetworkMaxSpeed { get; set; }
    public float ServerMaxSpeed { get; set; }


    protected Projectile()
    {
        Identifier = GetProjectileId(this);
    }
    
    public virtual void Init(ProjectileExtendedFactory projFactory)
    {
        _projFactory = projFactory;
        StateBuffer = new(10);
        MoveSpeed = _moveSpeed;
    }

    public virtual void Start(Character owner) { }
    
    public abstract void Tick(float delta);

    public virtual void Clear()
    {
        VirtualRenderTick = -1;
        StateBuffer.Clear();
        MoveSpeed = _moveSpeed;
    }
}