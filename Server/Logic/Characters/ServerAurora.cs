using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Data;
using ROMA2.Server.Logic.Extensions;
using Godot;

namespace ROMA2.Server.Logic.Characters;

public partial class ServerAurora : Aurora
{
    public override ClientInput Input
    {
        get
        {
            return _input;
        }
        set
        {
            if (value.IsPressed) RangedAttack();

            _input = value;
        }
    }

    public override void Init(ProjectileExtendedFactory factory)
    {
        base.Init(factory);

        _rangedAttackConfigId = ProjectilesConfiguration.GetProjectileId(typeof(AuroraRangedAttack));
    }

    public override void Tick(float delta)
    {
        Move(delta);
        CalculateCooldown(delta);
        HandleRangedAttackDelay(delta);
        
        this.SynchronizeTransform();
    }

    public void CalculateCooldown(float delta)
    {
        if (!_isRangedAttacking) _rangedAttackCurrentTime += delta;
    }

    private void HandleRangedAttackDelay(float delta)
    {
        if (!_isRangedAttacking) return;

        _rangedAttackDelayTimer += delta;

        if (_rangedAttackDelayTimer >= _rangedAttackDelay)
            ExecuteProjectileCreation();
    }

    public void RangedAttack()
    {
        if (_isRangedAttacking || _rangedAttackCurrentTime < _rangedAttackCooldown) return;

        _isRangedAttacking = true;
        AnimationState = AnimationState.Attack;
        _rangedAttackDelayTimer = 0f;
    }

    private void ExecuteProjectileCreation()
    {
        _projFactory
            .Create(_rangedAttackConfigId)
            .Start(this);

        _isRangedAttacking = false;
        AnimationState = AnimationState.Idle;
        _rangedAttackCurrentTime = 0f;
    }

    private void Move(float delta)
    {
        float velocityY = Velocity.Y;

        if (!IsOnFloor()) 
            velocityY -= _gravity * delta;
        else 
        {
            if (velocityY < 0)
                velocityY = 0;

            if (Input.IsJumped)
                velocityY = _jumpVelocity;
        }

        Vector3 forward = Input.MouseDirection;
        forward.Y = 0;
        forward = forward.Normalized();

        Vector3 right = forward.Cross(Vector3.Up).Normalized(); 

        Vector3 moveDirection = (forward * Input.WasdDirection.Y) + (right * Input.WasdDirection.X);

        if (moveDirection.LengthSquared() > 0)
            moveDirection = moveDirection.Normalized();

        Vector3 horizontalVelocity = moveDirection * MoveSpeed;

        Velocity = new Vector3(horizontalVelocity.X, velocityY, horizontalVelocity.Z);

        UpdateRotation(delta);

        switch (AnimationState)
        {
            case AnimationState.Idle when Velocity.LengthSquared() >= 0.25f:
                AnimationState = AnimationState.Walk;
                break;

            case AnimationState.Walk when Velocity.LengthSquared() < 0.25f:
                AnimationState = AnimationState.Idle;
                break;
        }

        MoveAndSlide();
    }

    private void UpdateRotation(float delta)
    {
        var direction2D = new Vector2(Input.MouseDirection.X, Input.MouseDirection.Z);

        if (direction2D.LengthSquared() < 0.0001f)
            return;

        direction2D = direction2D.Normalized();

        float targetRotationY = Mathf.Atan2(direction2D.X, direction2D.Y);

        float t = 1f - Mathf.Exp(-RotationSmoothing * delta);

        Vector3 currentRotation = Rotation;
        currentRotation.Y = Mathf.LerpAngle(currentRotation.Y, targetRotationY, t);
        Rotation = currentRotation;
    }
}