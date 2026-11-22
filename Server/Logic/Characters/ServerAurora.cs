using ROMA2.Shared.Characters;
using ROMA2.Shared.Configurations;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Data;
using ROMA2.Server.Logic.Extensions;
using ROMA2.Server.Logic.Projectiles;
using Godot;
using System.Collections.Generic;

namespace ROMA2.Server.Logic.Characters;

public partial class ServerAurora : Aurora
{
    [Export] public Area3D RazreziArea; 
    private readonly HashSet<Node3D> _hitTargetsThisCut = new();
    [Export] public float MinRequiredDot = 0.5f;

    public override ClientInput Input
    {
        get
        {
            return _input;
        }
        set
        {
            if (value.IsPressed) RangedAttack();
            if (value.IsKey1Pressed && !_input.IsKey1Pressed) Rpc(nameof(Razrezi));

            _input = value;
        }
    }

    protected override void Razrezi()
    {
        AnimationState = AnimationState.Cast;
        CastOrUseAbilityIndex = 1;

        _razreziProgressionTime = RAZREZI_PROGRESSION_TIME;
        _cutCountProgression = CutCount + DEFAULT_CUT_COUNT;
    }

    public override void Init(ProjectileExtendedFactory factory)
    {
        base.Init(factory);

        _rangedAttackConfigId = ProjectilesConfiguration.GetProjectileId(typeof(ServerAuroraRangedAttack));
    }

    protected override void OnTick(float delta)
    {
        CalculateCooldown(delta);
        HandleRangedAttackDelay(delta);
        HandleRazreziProgression(delta);
        this.Move(delta);
        
        this.SynchronizeTransform();
    }

    public void CalculateCooldown(float delta)
    {
        if (!_isRangedAttacking) _rangedAttackCurrentTime += delta;
        if (CastOrUseAbilityIndex == 1) UpdateRazreziAnimationCooldown(delta);
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
        var projectile = _projFactory.Create(_rangedAttackConfigId);

        WriteRangedAttackOffset(projectile);

        Rpc(nameof(SetHorizontalAttack), !HorizontalAttack);

        _isRangedAttacking = false;
        _rangedAttackCurrentTime = 0f;
        AnimationState = AnimationState.Idle;
    }

    private void HandleRazreziProgression(float delta)
    {
        if (CastOrUseAbilityIndex != 1 || _razreziAnimationEndCounter > RazreziEndTime) 
            return;

        if (_razreziProgressionTime < RAZREZI_PROGRESSION_TIME)
        {
            _razreziProgressionTime += delta;
            return;
        }
        else if (_cutCountProgression > 0)
        {
            _razreziProgressionTime = 0f;

            _hitTargetsThisCut.Clear();

            Vector3 forward = Input.MouseDirection.Normalized();

            // Получаем все тела, которые прямо сейчас пересекаются со сферой
            var bodies = RazreziArea.GetOverlappingBodies();

            foreach (Node3D body in bodies)
            {
                if (body == this || _hitTargetsThisCut.Contains(body)) 
                    continue;
                // Проверка команды:
                // if (body is Character enemy && enemy.Team == myTeam) continue;

                // Вектор от нас до цели (игнорируем разницу по высоте Y, если конус плоский)
                Vector3 direction = body.GlobalPosition - GlobalPosition;
                direction.Y = 0; 
                direction = direction.Normalized();

                float dotProduct = forward.Dot(direction);

                if (dotProduct >= MinRequiredDot)
                {
                    // Запоминаем цель, чтобы не ударить повторно за этот же взмах
                    _hitTargetsThisCut.Add(body);

                    if (dotProduct >= 0.9f)
                        GD.Print($"Чистый урон по: {body.Name}");
                    else
                        GD.Print($"Магический урон по: {body.Name}");
                }
            }

            _cutCountProgression--;
            
            //if (CutCount <= 4 && CutCount > 0) Rpc(nameof(SetCutCount), CutCount - 1);
        }
    }
}