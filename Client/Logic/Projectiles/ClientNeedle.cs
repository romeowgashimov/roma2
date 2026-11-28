using Godot;
using ROMA2.Client.Logic.Extensions;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Client.Logic.Projectiles;

public partial class ClientNeedle : Needle
{
    [Export] private MeshInstance3D _mesh;

    public override void Init(ProjectileExtendedFactory projFactory)
    {
        base.Init(projFactory);

        Synchronizer.Synchronized += this.OnNetworkTickReceived;

        DisableGlow();
    }

    public override void Tick(float delta)
    {
        this.InterpolateTransform(delta);
    }

    public override void Clear()
    {
        base.Clear();

        DisableGlow();
    }

    public void EnableGlow()
    {
        if (_mesh.GetActiveMaterial(0) is ShaderMaterial mat)
        {
            mat.SetShaderParameter("glow_strength", 2.5f);
            mat.SetShaderParameter("fresnel_glow_boost", 5f);
        }
    }

    public void DisableGlow()
    {
        if (_mesh.GetActiveMaterial(0) is ShaderMaterial mat)
        {
            mat.SetShaderParameter("glow_strength", 0f);
            mat.SetShaderParameter("fresnel_glow_boost", 0f);
        }
    }
}