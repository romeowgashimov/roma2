using System;
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

    protected readonly float _gravity = 9.8f; 
    protected readonly float _jumpVelocity = 4.5f;

    [Export] public Vector3 NetworkPosition { get; set; }
    [Export] public Vector3 NetworkRotation { get; set; }
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

    [Export] public float CurrentHealth { get; set; }
    public float MoveSpeed { get; set; } = 5;

    public long OwnerPeer { get; set; }
    public int Reference { get; set; }
    public AnimationState PreviousAnimationState { get; set; }
    public abstract ClientInput Input { get; set; }

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
    }

    public abstract void Tick(float delta);
}