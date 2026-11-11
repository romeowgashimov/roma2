using System;
using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Characters;

public abstract partial class Character : CharacterBody3D, INetworkTransform, ITickable
{
    protected ProjectileExtendedFactory _projFactory;
    [Export] protected float _rangedAttackCooldown = 0.5f;
    protected float _rangedAttackCurrentTime;
    [Export] protected float _rangedAttackDelay = 0.2f;
    protected bool _isRangedAttacking;
    protected float _rangedAttackDelayTimer;
    protected int _rangedAttackConfigId;
    protected AnimationState _animationState;
    protected ClientInput _input; 
    [Export] private float _moveSpeed = 5f;
    

    protected readonly float _gravity = 9.8f; 
    protected readonly float _jumpVelocity = 4.5f;

    [Export] public Vector3 NetworkPosition { get; set; }
    [Export] public Quaternion NetworkQuaternion { get; set; }
    [Export] public AnimationState AnimationState 
    { 
        get 
        { 
            return _animationState;
        }
        set
        {
            _animationState = value;

            AnimationStateChanged?.Invoke(value);
        }
    }
    [Export] public uint ServerTick { get; set; }

    [Export] public float CurrentHealth { get; set; }
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

    public long OwnerPeer { get; set; }
    public int Reference { get; set; }
    public AnimationState PreviousAnimationState { get; set; }
    public abstract ClientInput Input { get; set; }
    public List<NetworkState> StateBuffer { get; set; }
    public float VirtualRenderTick { get; set; } = -1;
    [Export] public float NetworkMaxSpeed { get; set; }
    public float ServerMaxSpeed { get; set; }
    [Export] public MultiplayerSynchronizer Synchronizer { get; set; }

    [Export] public float RotationSpeedRadPerSec = Mathf.Pi * 10f; 

    public event Action<AnimationState> AnimationStateChanged;

    public int GetCollisions()
    {
        int count = GetSlideCollisionCount();
        int added = 0;
        
        for (int i = 0; i < count; i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Character character)
            {
                // AddCollision(character);
                added++;
            }
        }
        return added;
    }

    public virtual void Init(ProjectileExtendedFactory projFactory)
    {
        _projFactory = projFactory;
        AnimationState = AnimationState.Idle;
        StateBuffer = new(10);
        MoveSpeed = _moveSpeed;
    }

    public abstract void Tick(float delta);
}