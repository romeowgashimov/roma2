using System;
using System.Collections.Generic;
using Godot;
using ROMA2.Server.Logic.Extensions;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Projectiles;

public partial class ServerTuman : Tuman
{
    private HashSet<Vector3> _usedNeedlesPositions = new();
    private float _currTime;

    public event Action<Vector3> TumanMoved;
    public event Action Died;

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        AreaEntered += OnAreaEntered;
        AreaExited += OnAreaExited;
        BodyExited += OnAreaExited;
    }

    public override void Tick(float delta)
    {
        base.Tick(delta);

        _currTime += delta;
        if (_currTime >= Lifetime) OnDied();

        this.SynchronizeTransform();
    }

    public void OnDied()
    {
        Died?.Invoke();
        
        // На всякий от утечки
        Died = null;

        _projFactory.Reset(this);
    }

    public override void Clear()
    {
        base.Clear();

        Initialized = false;
        _initCurrTime = 0;
        _currTime = 0f;
    }

    public void OnAreaEntered(Node3D node)
    {
        if (!Initialized) return;
    }

    public void OnAreaExited(Node3D node)
    {        
        if (!Initialized) return;
    
        if (node is Character aurora && CharacterOwner == aurora)
        {
            Position = CharacterOwner.Position;

            TumanMoved?.Invoke(Position);
        }
    }
}