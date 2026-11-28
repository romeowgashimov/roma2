using Godot.Collections;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using static ROMA2.Shared.Configurations.EffectConfiguration;

namespace ROMA2.Server.Logic.Effects;

public class SlowEffect : Effect
{
    public SlowEffect(Character sender, EffectSource source, int sourceIdentifier, float duration) : base(sender, source, sourceIdentifier, duration)
    {
    }

    
}