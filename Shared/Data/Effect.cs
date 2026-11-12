using ROMA2.Shared.Characters;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Data;

public abstract class Effect : ITickable
{
    public float EndTime { get; set; }
    public float CurrentTime { get; private set; }
    public bool IsEnd => CurrentTime >= EndTime;
    public bool IsRemoved { get; set; }

    public abstract void Init(Character character);

    public void Tick(float delta)
    {
        CurrentTime += delta;
        OnTick(delta);
    }

    protected abstract void OnTick(float delta);

    public abstract void Clear();
}
