using Godot;
using ROMA2.Server.Logic.Characters;
using ROMA2.Server.Logic.Extensions;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using ROMA2.Shared.Interfaces;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Server.Logic.Projectiles;

public partial class ServerNeedle : Needle, IDamageable
{
    private int _effectIdentifier; //= EffectFactory.GetEffectIdentifier(this);

    public void TakeDamage(Damage damage)
    {
        if (damage.Sender == CharacterOwner 
            && CharacterOwner is ServerAurora aurorar
            // && aurorar.GetEffect(_effectIdentifier) != null
            && damage.Source == DamageSource.MeleeAttack)
        {
            aurorar.AddNeedle(this);

            Rpc(nameof(Reset));
        }
    }

    public override void AddChild(int tumanReference)
    {
        base.AddChild(tumanReference);

        if (Tuman is ServerTuman tuman)
        {
            tuman.TumanMoved += OnTumanMoved;
            tuman.Died += RpcReset;
        }
    }

    public override void Reset()
    {
        base.Reset();

        if (Tuman is ServerTuman tuman)
        {
            tuman.TumanMoved -= OnTumanMoved;
            tuman.Died -= RpcReset;

            _projFactory.Reset(this);
        }
    }

    private void RpcReset()
    {
        Rpc(nameof(Reset));
    }

    public override void Start(Character owner)
    {
        CharacterOwner = owner;

        Clear();
    }

    public override void Tick(float delta)
    {
        this.SynchronizeTransform();
    }
    
    protected void OnTumanMoved(Vector3 newPosition)
    {
        Position = Tuman.Position + Offset;
    }
}