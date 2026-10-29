using System.Collections.Generic;
using Friflo.Engine.ECS;
using Godot;
using MemoryPack;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;
using ROMA2.Shared.Repositories;

namespace ROMA2.Shared.Projectiles;

public class ProjectileExtendedFactory 
{
    private Dictionary<int, Stack<Projectile>> _projectilePools;
    private EntityStore _store;
    private Node _parentRoot;
    protected NetworkBridge _bridge;
    protected ProjectilesConfiguration _projConfig;
    protected ProjectileRepository _repository;


    public ProjectileExtendedFactory(
        ProjectilesConfiguration projCong,
        EntityStore store,
        NetworkBridge bridge, 
        Node parentRoot,
        ProjectileRepository repository)
    {
        _projConfig = projCong;
        _projectilePools = new();
        _store = store;
        _bridge = bridge;
        _parentRoot = parentRoot;
        _repository = repository;
    }

    public virtual Projectile Spawn(byte[] data)
    {
        var creationData = MemoryPackSerializer.Deserialize<ProjectileCreationData>(data);
        int globalId = creationData.GlobalId;
        int localId = creationData.LocalId;

        if (_projectilePools.TryGetValue(globalId, out var projectiles))
        {
            if (projectiles.TryPop(out var projectile))
            {
                projectile.IsPooled = false;
                projectile.ProcessMode = Node.ProcessModeEnum.Inherit;
                projectile.Name = _projConfig.GetProjectileName(globalId) + $"_{localId}"; 
                projectile.Sync.RootPath = "..";

                ref var properties = ref projectile.Entity.GetComponent<ProjectileProperties>();
                properties.LocalId = localId;

                _repository.Add(localId, projectile);
                return projectile;
            }
        }
        else
        {
            _projectilePools[globalId] = new();
        }

        Projectile instance = _projConfig
                .GetProjectileScene(globalId)
                .Instantiate<Projectile>();
        
        instance.Name = _projConfig.GetProjectileName(globalId) + $"_{localId}";

        instance.Entity = _store.CreateEntity(
            new ProjectileProperties 
            { 
                GlobalId = globalId, 
                LocalId = localId,
                Reference = instance,
                DestroyDelay = 3f 
            });

        instance.Position = instance.Position with { Y = instance.Position.Y + 2 };

        _parentRoot.AddChild(instance);

        _repository.Add(localId, instance);

        return instance;
    }

    public virtual void Reset(int gloablId, int localId, Projectile projectile)
    {
        var sync = projectile.Sync;
        sync.RootPath = string.Empty;
        projectile.ProcessMode = Node.ProcessModeEnum.Disabled;
        _repository.Remove(localId);
        
        _projectilePools[gloablId].Push(projectile);
    }
}
