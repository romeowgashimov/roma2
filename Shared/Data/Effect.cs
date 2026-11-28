using System;
using Godot.Collections;
using MemoryPack;
using ROMA2.Shared.Characters;
using ROMA2.Shared.Interfaces;

namespace ROMA2.Shared.Data;

public abstract class Effect(
    Character sender, 
    EffectSource source, 
    int sourceIdentifier, 
    float duration) : ITickable
{
    private bool _end;
    private bool _removed;

    public float Duration { get; set; } = duration;
    public float CurrentTime { get; protected set; }
    public bool End 
    { 
        get => _end;
        set
        {
            if (Immutable && CurrentTime < Duration) return;

            if (!value) return;

            Ended?.Invoke(this);

            _end = value;
        }
    }
    public bool Immutable { get; protected set; }
    public int Identifier { get; init; }
    public EffectSource Source { get; set; } = source;
    // Какое конкретное умение или предмет спровоцировало эффект
    public byte SourceIdentifier { get; set; } = (byte)sourceIdentifier;
    public Character Sender { get; init; } = sender;
    public int SenderReference { get; init; } = sender.Reference;
    public Character Receiver { get; set; }
    public int ReceiverReference { get; set; }

    public event Action<Effect> Added;
    public event Action<Effect> Updated;
    public event Action<Effect> Ended;

    public Effect(
        Dictionary data, 
        Character sender) : this(sender, EffectSource.Unknown, byte.MaxValue, float.MaxValue)
    {
        Deserialize(data);
    }

    public Effect() : this(null, null)
    {
        
    }

    public void Init(Character receiver)
    {
        Receiver = receiver;

        OnInit();

        Added?.Invoke(this);
    }

    protected virtual void OnInit() { }

    public void Tick(float delta)
    {
        CurrentTime += delta;

        if (CurrentTime >= Duration) 
            End = true;

        OnTick(delta);
    }

    protected virtual void OnTick(float delta) { }

    public void Update()
    {
        OnUpdate();

        Updated?.Invoke(this);
    }

    protected virtual void OnUpdate() { }

    public virtual void Reset() { }

    public bool Equals(Effect effect)
    {
        if (Identifier == effect.Identifier
            && Sender == effect.Sender
            && Source == effect.Source 
            && SourceIdentifier == effect.SourceIdentifier)
            return true; 

        return false;
    }

    public bool Equals(int effectId, int senderRef, EffectSource source, int sourceId)
    {
        if (Identifier == effectId
            && SenderReference == senderRef
            && Source == source 
            && SourceIdentifier == sourceId)
            return true; 

        return false;
    }

    public virtual Dictionary Serialize()
    {
        return new Dictionary
        {
            { "effectId", Identifier },
            { "senderRef", Sender.Reference },
            { "source", (byte)Source },
            { "sourceId", SourceIdentifier },
            { "duration", Duration },
            { "currTime", CurrentTime },
            { "end", End }
        };
    }
    
    public virtual void Deserialize(Dictionary data)
    {
        Source = (EffectSource)(int)data["source"];
        SourceIdentifier = (byte)data["sourceId"];
        Duration = (float)data["duration"];
        CurrentTime = (float)data["currTime"];
        End = (bool)data["end"];
    }
    

    public static (int, int, EffectSource, int) DefaultDeserialize(Dictionary data)
    {
        int identifier = (int)data["effectId"];
        int senderRef = (int)data["senderRef"];
        EffectSource source = (EffectSource)(int)data["source"];
        int sourceId = (int)data["sourceId"];

        return (identifier, senderRef, source, sourceId);
    }
}

public abstract class Effect<TData> : Effect where TData : struct
{
    public void Deserialize(byte[] rawBytes)
    {
        TData data = MemoryPackSerializer.Deserialize<TData>(rawBytes);
        ApplyData(data);
    }

    protected abstract void ApplyData(TData data);
}

public enum EffectSource : byte
{
    Ability = 0,
    Item    = 1,

    Unknown = byte.MaxValue
}

public delegate void EffectModifier(Effect effect);
