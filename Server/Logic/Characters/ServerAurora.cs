using ROMA2.Shared.Characters;
using ROMA2.Shared.Projectiles;
using ROMA2.Shared.Data;
using static ROMA2.Shared.Configurations.ProjectilesConfiguration;
using static ROMA2.Shared.Configurations.EffectConfiguration;
using ROMA2.Server.Logic.Extensions;
using ROMA2.Server.Logic.Projectiles;
using Godot;
using System.Collections.Generic;
using Godot.Collections;
using ROMA2.Server.Logic.Effects;

namespace ROMA2.Server.Logic.Characters;

public partial class ServerAurora : Aurora
{
    [Export] public Area3D RazreziArea; 
    private readonly HashSet<Node3D> _hitTargetsThisCut = new();
    [Export] public float MinRequiredDot = 0.5f;

    // По-хорошему вынести все умения в отдельный абстрактный класс умений
    private readonly int _tumanId;
    private readonly int _needleId;
    private ServerTuman _activeTuman;
    private float _needleInitTime = 0.25f;
    // Сразу спавнится первая игла
    private float _needleCurrTime = 0.25f;
    [Export] public Vector3[] SpawnNeedlePositions = new Vector3[10]; 
    private int _spawnIndex;

    private int _effectIdentifier = GetEffectIdentifier(nameof(NeedleworkEffect));
    public readonly List<ServerNeedle> Needles = new();

    public override ClientInput Input
    {
        get
        {
            return _input;
        }
        set
        {
            if (value.IsPressed) RangedAttack();
            if (value.IsKey1Pressed && !_input.IsKey1Pressed) Razrezi();
            if (value.IsKey2Pressed && !_input.IsKey2Pressed) Tuman(); 
            if (value.IsKey3Pressed && !_input.IsKey3Pressed) Rivok(); 

            _input = value;
        }
    }

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        _effectModifiers.Add(SubscribeEffect);
    }

    protected void Rivok()
    {
        var effect = new NeedleworkEffect(
            sender: this, 
            EffectSource.Ability, 
            sourceIdentifier: 3,
            duration: 2);

        TakeEffect(effect);
    }

    protected void Razrezi()
    {
        AnimationState = AnimationState.Cast;
        CastOrUseAbilityIndex = 1;

        _razreziProgressionTime = RAZREZI_PROGRESSION_TIME;
        _cutCountProgression = CutCount + DEFAULT_CUT_COUNT;
    }

    protected void Tuman()
    {
        if (AnimationState == AnimationState.Cast || CastOrUseAbilityIndex == 2) return;

        AnimationState = AnimationState.Cast;
        CastOrUseAbilityIndex = 2;

        _activeTuman = (ServerTuman)ProjectileFactory.Create(_tumanId);
        
        _activeTuman.Start(this);

        _activeTuman.Died += Needles.Clear;

        _activeTuman.Position = Position;
    }

    private void HandleTumanProgression(float delta)
    {
        if (AnimationState == AnimationState.Cast 
            && CastOrUseAbilityIndex == 2)
        {
            if (!_activeTuman.Initialized)
            {
                if (_needleCurrTime < _needleInitTime) 
                    _needleCurrTime += delta;
                else
                {
                    _needleCurrTime = 0;

                    var needle = (ServerNeedle)ProjectileFactory.Create(_needleId);

                    needle.Rpc(nameof(needle.AddChild), _activeTuman.Reference);

                    needle.Offset = SpawnNeedlePositions[_spawnIndex];

                    needle.Position = needle.Offset + _activeTuman.Position;

                    Needles.Add(needle);

                    _spawnIndex++;
                }

                if (!Input.IsKey2Pressed) 
                {
                    _activeTuman.OnDied();

                    _spawnIndex = 0;
                    
                    AnimationState = AnimationState.Idle;
                    CastOrUseAbilityIndex = byte.MinValue;
                }
            }
            else
            {
                _spawnIndex = 0;
                _activeTuman = null;

                AnimationState = AnimationState.Idle;
                CastOrUseAbilityIndex = byte.MinValue;
            }
        }
    }

    protected ServerAurora()
    {
        _rangedAttackConfigId = GetProjectileId(typeof(ServerAuroraRangedAttack));
        _tumanId = GetProjectileId(typeof(Tuman));
        _needleId = GetProjectileId(typeof(Needle));
    }

    protected override void OnTick(float delta)
    {
        CalculateCooldown(delta);
        HandleRangedAttackDelay(delta);
        HandleRazreziProgression(delta);
        HandleTumanProgression(delta);
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
        if (_isRangedAttacking 
            || _rangedAttackCurrentTime < _rangedAttackCooldown
            || AnimationState == AnimationState.Cast) return;

        _isRangedAttacking = true;
        AnimationState = AnimationState.Attack;
        _rangedAttackDelayTimer = 0f;
    }

    private void ExecuteProjectileCreation()
    {
        var projectile = ProjectileFactory.Create(_rangedAttackConfigId);

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