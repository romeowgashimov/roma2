using System;
using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Characters;

public abstract partial class Character : CharacterBody3D, INetworkTransform, ITickable, IDamageable, IEffectable
{
    protected ProjectileExtendedFactory _projFactory;
    [Export] protected float _rangedAttackCooldown = 0.5f;
    protected float _rangedAttackCurrentTime;
    [Export] protected float _rangedAttackDelay = 0.2f;
    protected bool _isRangedAttacking;
    protected float _rangedAttackDelayTimer;
    protected int _rangedAttackConfigId;
    private AnimationState _animationState;
    protected float _rangedAttackAnimationCounter;
    protected ClientInput _input; 
    protected List<DamageCalculator> _damageCalculators;
    private bool _isTicking;
    private readonly List<Effect> _effects = new();
    private readonly List<Effect> _toAdd = new();

    protected readonly float _gravity = 9.8f; 
    protected readonly float _jumpVelocity = 4.5f;

    [Export] public float RotationSmoothing = 10f;

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
    [Export] public float MoveSpeed { get; set; } = 5;

    public long OwnerPeer { get; set; }
    public int Reference { get; set; }
    public AnimationState PreviousAnimationState { get; set; }
    public abstract ClientInput Input { get; set; }
    public List<NetworkState> StateBuffer { get; set; }
    public float VirtualRenderTick { get; set; } = -1;
    [Export] public MultiplayerSynchronizer Synchronizer { get; set; }

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
    }

    public virtual void Tick(float delta)
    {
        _isTicking = true;

        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            var effect = _effects[i];
            if (effect.IsRemoved)
            {
                // Быстрое удаление O(1): ставим последний элемент на место удаляемого
                int lastIndex = _effects.Count - 1;
                _effects[i] = _effects[lastIndex];
                _effects.RemoveAt(lastIndex);

                effect.Clear();
            }
            else effect.Tick(delta);

            if (effect.IsEnd)
            {
                // Быстрое удаление O(1): ставим последний элемент на место удаляемого
                int lastIndex = _effects.Count - 1;
                _effects[i] = _effects[lastIndex];
                _effects.RemoveAt(lastIndex);

                effect.Clear();
            }
        }

        _isTicking = false;

        if (_toAdd.Count > 0)
        {
            _effects.AddRange(_toAdd);
            _toAdd.Clear();
        }
    }


    public void TakeDamage(Damage damage)
    {
        foreach (var calculator in _damageCalculators)
        {
            calculator(ref damage);

            if (damage.Amount <= 0)
                break;
        }

        if (damage.Amount > 0)
            ApplyFinalDamage(damage);
    }

    private void ApplyFinalDamage(Damage damage)
    {
        int finalDamage = (int)Math.Clamp(damage.Amount, 0, CurrentHealth);
        CurrentHealth -= finalDamage;

        if (damage.Sender is Character character)
            // Мб, сделать интерфейс для тех, кому нужно отправлять обработанный урон,
            // условным вампризмом могут обладать не только персонажи
            // character.SendFinalDamage(damage); 

        if (CurrentHealth <= 0)
            QueueFree();
    }

    public void TakeEffect(Effect effect)
    {
        if (_isTicking) _toAdd.Add(effect);
        else _effects.Add(effect);
    }
}