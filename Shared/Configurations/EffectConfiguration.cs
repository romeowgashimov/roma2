using System;
using System.Collections.Generic;

namespace ROMA2.Shared.Configurations;

public static class EffectConfiguration
{
    // Имена эффектов должны совпадать с именами классов эффектов
    private static Dictionary<string, int> _ids = new()
    {
        { "SlowEffect", 1 },
        { "StunEffect", 2 },
        { "NeedleworkEffect", 3 }
    };

    public static int GetEffectIdentifier(string name)
    {
        if (_ids.TryGetValue(name, out int identifier))
            return identifier;

        throw new Exception($"Идентификатор эффекта {name} не найден");
    }
}