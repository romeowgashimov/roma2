using ROMA2.Shared.Characters;

namespace ROMA2.Shared.Data;

public record struct Damage(
    Character Sender, //Отправитель
    Character Receiver, // Получатель
    DamageSource Source, // Источник
    int AbilityIdentifier, // Идентификатор проджектайла или умения, нанёсшего урон
    DamageType Type, // Тип урона
    int Amount); // Количество урона

public enum DamageSource : byte
{
    MeleeAttack,
    ChargedMeleeAttack,
    RangedAttack,
    Ability,

    Environment
}

public enum DamageType : byte
{
    Physical,
    Magical,
    Pure
}

public delegate void DamageCalculator(ref Damage damage);