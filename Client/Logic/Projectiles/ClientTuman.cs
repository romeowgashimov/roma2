using Godot;
using ROMA2.Client.Logic.Extensions;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Client.Logic.Projectiles;

public partial class ClientTuman : Tuman
{
    [Export] private MeshInstance3D _mesh;

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        Synchronizer.Synchronized += this.OnNetworkTickReceived;

        StartAnimation();
    }

    public override void Tick(float delta)
    {
        base.Tick(delta);

        this.InterpolateTransform(delta);
    }

    public override void Clear()
    {
        base.Clear();

        StartAnimation();
    }

    private void StartAnimation()
    {
        if (_mesh.GetActiveMaterial(0) is ShaderMaterial mat)
		{
			// Сбрасываем в 0 перед стартом
			mat.SetShaderParameter("dissolve_amount", 1f);
			
			CreateTween()
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut)
                .TweenProperty(mat, "shader_parameter/dissolve_amount", 0.3f, InitTime);
		}
    }
}