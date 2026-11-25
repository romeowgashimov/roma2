using Godot;
using ROMA2.Shared.Characters;

namespace ROMA2.Shared.Projectiles;

public abstract partial class Tuman : Projectile
{
    public float InitTime = 1.5f;
    public float Lifetime { get; protected set; } = 5f; 
    
    protected float _initCurrTime;

    public float InitCurrTime => _initCurrTime;

    public bool Initialized = false;

    public override void Tick(float delta)
    {
        if (!Initialized)
        {
            if (_initCurrTime < InitTime)
                _initCurrTime += delta;
            else Initialized = true;

            return;
        }
    }

    public override void Start(Character owner)
    {
        CharacterOwner = owner;
    }
}