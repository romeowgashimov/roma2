using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Shared;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Core;

public class MovementSystem : QuerySystem<CharacterReference, InputSynchronizerReference, MoveSpeed>
{
    // Гравитацию для 3D лучше приблизить к реальной (например, 9.8f или около того)
    private readonly float _gravity = 9.8f; 

    protected override void OnUpdate()
{
    Query.ForEachEntity((ref CharacterReference charRef, ref InputSynchronizerReference syncRef, ref MoveSpeed moveSpeed, Entity entity) =>
    {
        Character character = charRef.Value;
        InputSynchronizer sync = syncRef.Value;

        if (character == null || sync == null) return;

        // 1. Получаем текущую вертикальную скорость персонажа
        float velocityY = character.Velocity.Y;

        // 2. Применяем гравитацию, если персонаж в воздухе
        if (!character.IsOnFloor())
        {
            // Убедитесь, что у вас есть глобальный доступ к Delta времени сервера (например, через ваш класс Tick или GetProcessDeltaTime)
            float dt = (float)character.GetProcessDeltaTime(); 
            velocityY -= _gravity * dt; 
        }
        else if (velocityY < 0)
        {
            velocityY = 0; // Сбрасываем накопленную гравитацию на земле
        }

        // --- 3. РАСЧЕТ НАПРАВЛЕНИЯ ДВИЖЕНИЯ В СТИЛЕ DEADLOCK ---
        
        // Вектор вперед — это направление мыши/камеры
        Vector3 forward = sync.MouseDirection;
        forward.Y = 0; // Обнуляем Y, чтобы персонаж не взлетал/не зарывался в землю при взгляде вверх/вниз
        forward = forward.Normalized();

        // Находим вектор "вправо" (перпендикулярно вектору вперед и мировой оси Up)
        Vector3 right = forward.Cross(Vector3.Up).Normalized(); 
        // Примечание: в зависимости от знака Cross, право и лево могут поменяться. 
        // Если инвертировано, используйте: Vector3.Up.Cross(forward)

        // sync.WasdDirection: X — это влево/вправо, Y — это вперед/назад
        Vector3 moveDirection = (forward * sync.WasdDirection.Y) + (right * sync.WasdDirection.X);
        
        // Нормализуем итоговый вектор направления движения, чтобы по диагонали персонаж не бегал быстрее
        if (moveDirection.LengthSquared() > 0)
        {
            moveDirection = moveDirection.Normalized();
        }

        // Умножаем направление на скорость
        Vector3 horizontalVelocity = moveDirection * moveSpeed.Value;

        // 4. Собираем итоговый вектор скорости (горизонтальная скорость + гравитация)
        character.Velocity = new Vector3(horizontalVelocity.X, velocityY, horizontalVelocity.Z);

        // 5. Двигаем персонажа
        character.MoveAndSlide();
    });
}
}
