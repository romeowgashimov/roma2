using System.Collections.Generic;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Shared.Repositories;

public class ProjectileRepository
{
    private Dictionary<int, Projectile> _ingameProjectiles = [];

    public void Add(int localId, Projectile projectile)
    {
        _ingameProjectiles[localId] = projectile;
    }

    public Projectile Remove(int localId)
    {
        var projectile = _ingameProjectiles[localId];
        _ingameProjectiles.Remove(localId);
        return projectile;
    }

    public Projectile Get(int localId)
    {
        return _ingameProjectiles[localId];
    }
}