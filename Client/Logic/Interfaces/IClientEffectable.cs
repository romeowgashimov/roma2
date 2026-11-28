using ROMA2.Client.Logic.Effects;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Client.Logic.Interfaces;

public interface IClientEffectable : IEffectable
{
    public EffectFactory EffectFactory { get; set; }
}