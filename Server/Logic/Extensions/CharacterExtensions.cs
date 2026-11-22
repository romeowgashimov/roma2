using Godot;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Data;

namespace ROMA2.Server.Logic.Extensions;

public static class CharacterExtensions
{
    public static void Move(this Character character, float delta)
    {
        if (character.AnimationState == AnimationState.Cast) return;

        float velocityY = character.Velocity.Y;

        if (!character.IsOnFloor()) 
            velocityY -= character.Gravity * delta;
        else 
        {
            if (velocityY < 0)
                velocityY = 0;

            if (character.Input.IsJumped)
                velocityY = character.JumpVelocity;
        }

        Vector3 forward = character.Input.MouseDirection;
        forward.Y = 0;
        forward = forward.Normalized();

        Vector3 right = forward.Cross(Vector3.Up); 

        Vector3 moveDirection = (forward * character.Input.WasdDirection.Y) + (right * character.Input.WasdDirection.X);

        if (moveDirection.LengthSquared() > 0)
            moveDirection = moveDirection.Normalized();

        Vector3 horizontalVelocity = moveDirection * character.MoveSpeed;

        character.Velocity = new Vector3(horizontalVelocity.X, velocityY, horizontalVelocity.Z);

        character.UpdateRotation(delta);

        switch (character.AnimationState)
        {
            case AnimationState.Idle when character.Velocity.LengthSquared() >= 0.25f:
                character.AnimationState = AnimationState.Walk;
                break;

            case AnimationState.Walk when character.Velocity.LengthSquared() < 0.25f:
                character.AnimationState = AnimationState.Idle;
                break;
        }

        character.MoveAndSlide();
    }

    private static void UpdateRotation(this Character character, float delta)
    {
        var direction2D = new Vector2(character.Input.MouseDirection.X, character.Input.MouseDirection.Z);

        if (direction2D.LengthSquared() < 0.0001f)
            return;

        direction2D = direction2D.Normalized();

        float targetRotationY = Mathf.Atan2(direction2D.X, direction2D.Y);

        float t = 1f - Mathf.Exp(-character.RotationSmoothing * delta);

        Vector3 currentRotation = character.Rotation;
        currentRotation.Y = Mathf.LerpAngle(currentRotation.Y, targetRotationY, t);
        character.Rotation = currentRotation;
    }
}