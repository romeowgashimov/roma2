using System.Collections.Generic;
using ROMA2.Shared.Data;

namespace ROMA2.Shared.Interfaces;

public interface IEffectable
{
    void TakeEffect(Effect effect);
    
    IEnumerable<Effect> GetEffects(int identifier); 

    bool HasEffect(int identifier);
}