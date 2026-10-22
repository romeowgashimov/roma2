using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using Godot;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;
using ROMA2.Shared.Network;

namespace ROMA2.Server.Logic.Systems;

public class MovementSystem : QuerySystem<
        CharacterReference, 
        InputSynchronizerReference, 
        MoveSpeed, 
        AnimationStateComponent>
{
    // Гравитацию для 3D лучше приблизить к реальной (например, 9.8f или около того)
    private readonly float _gravity = 9.8f; 
    private readonly float _jumpVelocity = 4.5f;

    protected override void OnUpdate()
{
    Query.ForEachEntity((ref CharacterReference charRef, 
                         ref InputSynchronizerReference syncRef, 
                         ref MoveSpeed moveSpeed, 
                         ref AnimationStateComponent animState, Entity entity) =>
    {
        Character character = charRef.Value;
        InputSynchronizer sync = syncRef.Value;

        if (character == null || sync == null) return;

        // 1. Получаем текущую вертикальную скорость персонажа
        float velocityY = character.Velocity.Y;

        // 2. Применяем гравитацию, если персонаж в воздухе
        if (!character.IsOnFloor())
        {
            float dt = (float)character.GetProcessDeltaTime(); 
            velocityY -= _gravity * dt; 
        }
        else 
        {
            if (velocityY < 0)
            {
                velocityY = 0; // Сбрасываем накопленную гравитацию на земле
            }
            if (sync.IsJumped) 
            {
                velocityY = _jumpVelocity;
            }
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

        character.NetworkPosition = character.Position;

        // Проектируем направление на плоскость XZ (чтобы наклон камеры вверх/вниз не ломал вращение)
        Vector2 direction2D = new Vector2(sync.MouseDirection.X, sync.MouseDirection.Z).Normalized();

        // Находим абсолютный угол в радианах. 
        // В Godot угол 0 смотрит по оси Vector3.Forward (обычно -Z), поэтому используем Mathf.Atan2
        float targetRotationY = Mathf.Atan2(direction2D.X, direction2D.Y);

        // Жестко перезаписываем вращение (без оператора +=)
        Vector3 currentRotation = character.NetworkRotation;
        currentRotation.Y = targetRotationY;
        character.NetworkRotation = currentRotation;

        if (character.Velocity.LengthSquared() >= 0.5)
            animState.Current = AnimationState.Walk;


        // 5. Двигаем персонажа
        character.MoveAndSlide();
    });
}
}
