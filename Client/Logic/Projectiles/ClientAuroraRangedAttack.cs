using ROMA2.Client.Logic.Extensions;
using ROMA2.Shared.Projectiles;

namespace ROMA2.Client.Logic.Projectiles;

public partial class ClientAuroraRangedAttack : AuroraRangedAttack
{
    public override void Clear()
    {
        //
    }

    public override void Tick(float delta)
    {
        this.InterpolateTransform(delta);
    }
}