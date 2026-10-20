using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Client.Logic.Data;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Systems;

public class TransformInterpolationSystem : QuerySystem<CharacterReference>
{
    public TransformInterpolationSystem()
    {
        Filter.AllTags(Tags.Get<InterpolateTag>());
    }

    protected override void OnUpdate()
    {
        Query.ForEachEntity((ref CharacterReference charRef, Entity entity) =>
        {
            Character character = charRef.Value;
            // Проверяем, что объект существует и инициализирован в Godot
            if (character == null) return;

            float dt = (float)character.GetProcessDeltaTime();
            float t = 10f * dt;

            // 1. Берем текущий трансформ объекта
            Transform3D currentTransform = character.GlobalTransform;

            // 2. Создаем целевой трансформ
            Transform3D targetTransform = currentTransform;
            targetTransform.Origin = character.NetworkPosition; // Задаем целевую позицию
            targetTransform.Basis = new Basis(Quaternion.FromEuler(character.NetworkRotation)); // Задаем целевое вращение

            // 3. Интерполируем всё вместе и применяем обратно к объекту
            character.GlobalTransform = currentTransform.InterpolateWith(targetTransform, t);
        });
    }
}
