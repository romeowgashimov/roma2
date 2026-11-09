using Godot;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Projectiles;

public partial class ServerAuroraRangedAttack : AuroraRangedAttack
{
    [Export] private float _lifetime;
    private float _currentTime;

    public override void Clear()
    {
        _currentTime = 0;
    }

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        Clear();
    }

    public override void Tick(float delta)
    {
        _currentTime += delta;

        if (_currentTime >= _lifetime)
            _projFactory.Reset(Identifier, Reference, this);

        NetworkPosition = Position;
        NetworkRotation = Rotation;
    }
}