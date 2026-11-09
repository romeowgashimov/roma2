using System;
using System.Collections.Generic;
using Godot;
using MemoryPack;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Shared.Projectiles;

public abstract class ProjectileExtendedFactory 
{
    private Dictionary<int, Stack<Projectile>> _projectilePools;
    private Node _parentRoot;
    protected NetworkBridge _bridge;
    protected ProjectileRepository _repository;

    public ProjectilesConfiguration ProjConfig;

    public ProjectileExtendedFactory(
        ProjectilesConfiguration projCong,
        NetworkBridge bridge, 
        Node parentRoot,
        ProjectileRepository repository)
    {
        ProjConfig = projCong;
        _projectilePools = new();
        _bridge = bridge;
        _parentRoot = parentRoot;
        _repository = repository;
    }

    public virtual Projectile Spawn(byte[] data)
    {
        var creationData = MemoryPackSerializer.Deserialize<ProjectileCreationData>(data);
        int globalId = creationData.GlobalId;
        int reference = creationData.LocalId;

        if (_projectilePools.TryGetValue(globalId, out var projectiles))
        {
            if (projectiles.TryPop(out var projectile))
            {
                projectile.ProcessMode = Node.ProcessModeEnum.Inherit;
                projectile.Name = ProjConfig.GetProjectileName(globalId) + $"_{reference}"; 
                projectile.Sync.RootPath = "..";

                projectile.Reference = reference;

                projectile.Clear();

                _repository.Add(reference, projectile);
                return projectile;
            }
        }
        else
        {
            _projectilePools[globalId] = new();
        }

        Projectile instance = ProjConfig
                .GetProjectileScene(globalId)
                .Instantiate<Projectile>();
        
        instance.Name = ProjConfig.GetProjectileName(globalId) + $"_{reference}";

        instance.Reference = reference;

        instance.Position = instance.Position with { Y = instance.Position.Y + 2 };

        _parentRoot.AddChild(instance);

        _repository.Add(reference, instance);

        return instance;
    }

    public virtual void Reset(int gloablId, int reference, Projectile projectile)
    {
        var sync = projectile.Sync;
        sync.RootPath = string.Empty;
        projectile.ProcessMode = Node.ProcessModeEnum.Disabled;
        _repository.Remove(reference);
        
        _projectilePools[gloablId].Push(projectile);
    }
}
