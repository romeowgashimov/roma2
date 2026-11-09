using System.Collections.Generic;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Repositories;

public class CharacterRepository : ITickable
{
    private Dictionary<long, Character> _characters = [];

    public void Add(long reference, Character character)
    {
        _characters.Add(reference, character);
    }

    public void Remove(long reference)
    {
        _characters.Remove(reference);
    }

    public Character Get(long reference)
    {
        return _characters[reference];
    }

    public void Tick(float delta)
    {
        foreach (var pair in _characters)
            pair.Value.Tick(delta);
    }
}