using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Shared.Interfaces;

public interface INetworkTransform
{
    /// <summary>
    /// Задержка интерполяции в тиках (~50 мс при 60 Гц).
    /// </summary>
    const float INTERPOLATION_DELAY_TICKS = 2f;

    /// <summary>
    /// Тикрейт СЕРВЕРА.
    /// </summary>
    const float TICK_RATE = 60f;

    /// <summary>
    /// Коэффициент подгонки виртуального рендер-тика (time-sync). 0.1 = мягко.
    /// </summary>
    const float TIME_SYNC_GAIN = 0.1f;

    /// <summary>
    /// Насколько тиков рендер-тик может обогнать самый свежий тик в буфере. 
    /// </summary>
    const float MAX_EXTRAPOLATION_TICKS = 2f;

    /// <summary>
    /// Допуск экстраполяции фактора t — ВТОРИЧНЫЙ предел (основной — по дистанции).
    /// </summary>
    const float MAX_EXTRAPOLATION_SLACK = 0.25f;

    /// <summary>
    /// Страховка от разрастания буфера при бурстах пакетов. 
    /// </summary>
    const int MAX_BUFFER_STATES = 32;

    /// <summary>
    /// Максимальная скорость персонажа, м/с — ДЕФОЛТ.
    /// </summary>
    const float MAX_SPEED_FALLBACK = 5f;

    const float TIME_SYNC_DEADZONE = 0.25f;
    const float MIN_TIME_SCALE = 0.97f;
    const float MAX_TIME_SCALE = 1.03f;
    const float TELEPORT_THRESHOLD = 15.0f;

    /// <summary>
    /// Актуальная максимальная скорость конкретного персонажа. 
    /// <= 0 — использовать MAX_SPEED_FALLBACK.
    /// </summary>
    float NetworkMaxSpeed { get; set; }

    /// <summary>
    /// Текущая максимальная скорость на сервере, 
    /// Нужно, чтобы сервер постоянно не отправлял одно и то же значение.
    /// </summary>
    float ServerMaxSpeed { get; set; }

    uint ServerTick { get; set; }
    float VirtualRenderTick { get; set; }

    Vector3 NetworkPosition { get; set; }
    Quaternion NetworkQuaternion { get; set; }

    Vector3 Position { get; }
    Quaternion Quaternion { get; }
    Transform3D GlobalTransform { get; set; }

    List<NetworkState> StateBuffer { get; set; }

    MultiplayerSynchronizer Synchronizer { get; set; }
}
