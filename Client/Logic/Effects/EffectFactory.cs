using System;
using System.Collections.Generic;
using ROMA2.Shared.Data;
using GodotDict = Godot.Collections.Dictionary;
using static ROMA2.Shared.Configurations.EffectConfiguration;
using ROMA2.Shared.Repositories;
using ROMA2.Shared.Characters;

namespace ROMA2.Client.Logic.Effects;

public class EffectFactory(CharacterRepository charRepo)
{
    private Dictionary<int, Func<GodotDict, Character, Effect>> _factories = new()
    {
        { GetEffectIdentifier(nameof(NeedleworkEffect)), (data, sender) => new NeedleworkEffect(data, sender) }
    };
    private CharacterRepository _charRepo = charRepo;

    public bool TryCreate(GodotDict data, out Effect effect)
    {
        int identifier = (int)data["effectId"];
        int senderRef = (int)data["senderRef"];

        if (_factories.TryGetValue(identifier, out var factory))
        {
            effect = factory(data, _charRepo.Get(senderRef));

            return true;
        }

        effect = null;
        return false;
    }
}