using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using ROMA2.Client.Logic.Projectiles;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using static ROMA2.Shared.Configurations.EffectConfiguration;

namespace ROMA2.Client.Logic.Effects;

public class NeedleworkEffect : Effect
{
    private Array _needleRefs;
    private List<ClientNeedle> _needles = new();

    public NeedleworkEffect(Dictionary data, Character sender) : base(data, sender)
    {
        Identifier = GetEffectIdentifier(nameof(NeedleworkEffect));

        GetNeedlesRefs(data);
    }

    public override void Reset()
    {
        foreach (var needle in _needles)
            needle.DisableGlow();

        _needleRefs?.Dispose();
        _needles.Clear();
    }

    protected override void OnInit()
    {
        var projFactory = Sender.ProjectileFactory;

        foreach (int needleRef in _needleRefs.Select(v => (int)v))
        {
            ClientNeedle needle = (ClientNeedle)projFactory.GetActiveProjectile(needleRef);

            _needles.Add(needle);

            needle.EnableGlow();
        }
    }

    public override void Deserialize(Dictionary data)
    {
        base.Deserialize(data);

        GetNeedlesRefs(data);

        OnInit();
    }

    private void GetNeedlesRefs(Dictionary data)
    {
        _needleRefs?.Dispose();

        _needleRefs = (Array)data["refs"];
    }
}