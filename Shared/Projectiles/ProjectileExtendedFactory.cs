using System;
using System.Collections.Generic;
using Godot;

namespace ROMA2.Shared.Projectiles;

public partial class ProjectileExtendedFactory : Node
{
    private readonly MultiplayerSpawner _spawner;
    
    // Хранилище пулов, где ключ — тип снаряда
    private readonly Dictionary<Type, Stack<Projectile>> _objectPool = new();

    // Карта для сетевой синхронизации: ID (int) -> Type
    // Это необходимо, так как передавать System.Type через Godot Variant напрямую нельзя
    private readonly Dictionary<int, Type> _idToTypeMap = new();
    private readonly Dictionary<Type, int> _typeToIdMap = new();

    public ProjectileExtendedFactory(MultiplayerSpawner spawner)
    {
        _spawner = spawner;
        
        // Регистрируем типы снарядов (можно автоматизировать через рефлексию)
        RegisterProjectile<AuroraRangedAttack>(id: 1);

        // Godot ожидает, что функция принимает Variant (наши сетевые данные) и возвращает Node
        _spawner.SpawnFunction = Callable.From<Variant, Node>(SpawnFromServerRequest);
    }

    // 1. Публичный метод для запуска спавна на сервере (через дженерик)
    public T SpawnFromPool<T>(Vector3 position, Vector3 direction) where T : Projectile, new()
    {
        if (!_typeToIdMap.TryGetValue(typeof(T), out int typeId))
        {
            GD.PrintErr($"Тип {typeof(T).Name} не зарегистрирован в фабрике!");
            return null;
        }

        // Вызываем сетевой спавн Godot. Передаем ID типа. 
        // Если нужно передать еще и позицию, можно использовать Array: new Godot.Collections.Array { typeId, position }
        T spawnedNode = (T)_spawner.Spawn(typeId);
        
        // Дополнительная настройка объекта после спавна на сервере
        spawnedNode.GlobalPosition = position;
        // spawnedNode.SetDirection(direction); 
        
        return spawnedNode;
    }

    // 2. Внутренний метод, который Godot автоматически вызывает на сервере и ВСЕХ клиентах
    private Node SpawnFromServerRequest(Variant data)
    {
        int typeId = data.AsInt32();

        if (!_idToTypeMap.TryGetValue(typeId, out Type projectileType))
        {
            GD.PrintErr($"Получен неизвестный Type ID снаряда: {typeId}");
            return null;
        }

        // Достаем из пула по типу (вынесено в дженерик-метод ниже)
        return GetOrCreateFromPool(projectileType);
    }

    // 3. Дженерик-логика работы с пулом
    private Projectile GetOrCreateFromPool(Type type)
    {
        // Если пула для такого типа еще нет, создаем стек
        if (!_objectPool.TryGetValue(type, out var stack))
        {
            stack = new Stack<Projectile>();
            _objectPool[type] = stack;
        }

        // Если в пуле есть готовый объект — возвращаем его
        if (stack.Count > 0)
        {
            Projectile reusableProjectile = stack.Pop();
            
            // Важно: Godot перенесет объект в дерево сцен автоматически,
            // но нам нужно сбросить его состояние (например, включить видимость/процессинг)
            reusableProjectile.ProcessMode = ProcessModeEnum.Inherit; 
            return reusableProjectile;
        }

        // Если пул пуст — создаем новый экземпляр через активатор
        return (Projectile)Activator.CreateInstance(type);
    }

    // Метод регистрации типов снарядов
    private void RegisterProjectile<T>(int id) where T : Projectile
    {
        Type type = typeof(T);
        _idToTypeMap[id] = type;
        _typeToIdMap[type] = id;
    }
}
