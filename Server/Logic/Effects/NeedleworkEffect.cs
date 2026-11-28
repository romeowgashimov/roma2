using Godot.Collections;
using ROMA2.Server.Logic.Characters;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using static ROMA2.Shared.Configurations.EffectConfiguration;

namespace ROMA2.Server.Logic.Effects;

public class NeedleworkEffect : Effect
{
    public NeedleworkEffect(
        Character sender,
        EffectSource source,
        byte sourceIdentifier,
        float duration) : base(sender, source, sourceIdentifier, duration)
    {
        Identifier = GetEffectIdentifier(nameof(NeedleworkEffect));
    }

    public override Dictionary Serialize()
    {
        var data = base.Serialize();

        var needleRefs = new Array();

        if (Sender is ServerAurora aurora)
        {
            foreach (var needle in aurora.Needles)
                needleRefs.Add(needle.Reference);
        }

        data.Add("refs", needleRefs);

        return data;
    }

    protected override void OnUpdate()
    {
        CurrentTime = 0;
    }
}