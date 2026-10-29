using System;
using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Configurations;

public class ProjectilesConfiguration
{
    private Dictionary<int, string> _projectilesPaths = new()
    {
        { 1, "AuroraRangedAttack" },
    };
    private static Dictionary<Type, int> _ids = new()
    {
        { typeof(AuroraRangedAttack), 1 }
    };
    private Dictionary<int, PackedScene> _cachedProjectiles;
    private bool _isServer;

    private int _projectilesCount;

    public int ProjectilesCount
    {
        get
        {
            _projectilesCount++;
            return _projectilesCount;
        }
        private set
        {
            _projectilesCount = value;
        }
    }


    public ProjectilesConfiguration(bool isServer)
    {
        _isServer = isServer;
        _cachedProjectiles = new();
    }

    public PackedScene GetProjectileScene(int id)
    {
        if (_cachedProjectiles.TryGetValue(id, out var packedScene))
            return packedScene;

        string path = _isServer 
            ? "res://Server/Scenes/" 
            : "res://Client/Scenes/";
        path += $"{_projectilesPaths[id]}.tscn";
        
        packedScene = GD.Load<PackedScene>(path);

        _cachedProjectiles.TryAdd(id, packedScene);

        return packedScene;
    }

    public static int GetProjectileId<T>(T projectile) where T : Projectile
    {
        Type currentType = projectile.GetType();

        return GetProjectileId(currentType);
    }

    public static int GetProjectileId(Type type)
    {
        foreach (var pair in _ids)
            if (pair.Key.IsAssignableFrom(type))
                return pair.Value;

        throw new Exception($"Текущий тип: {type.Name} не зарегистрирован");
    }

    public string GetProjectileName(int id)
    {
        return _projectilesPaths[id];
    }
}