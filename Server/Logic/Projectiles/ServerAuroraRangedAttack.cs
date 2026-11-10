using Godot;
using ROMA2.Server.Logic.Extensions;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Projectiles;

public partial class ServerAuroraRangedAttack : AuroraRangedAttack
{
    [Export] private float _lifetime;
    private float _currentTime;
    
    public float MoveSpeed = 10;
    public Vector3 Direction;

    public override void Clear()
    {
        _currentTime = 0;
    }

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        Clear();
    }

    public override void Start(Character owner)
    {
        CharacterOwner = owner;

        Direction = CharacterOwner.Input.MouseDirection;

        if (CharacterOwner is Aurora aurora)
            aurora.WriteRangedAttackOffset(this);
        
        Clear();
    }

    public override void Tick(float delta)
    {
        Move(delta);

        _currentTime += delta;

        if (_currentTime >= _lifetime)
            _projFactory.Reset(Identifier, Reference, this);

        this.SynchronizeTransform();
    }

    public void Move(float delta)
    {
        Position += Direction * MoveSpeed * delta;
    }
}