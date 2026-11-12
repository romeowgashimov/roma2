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
    /// Страховка от разрастания буфера при бурстах пакетов.
    /// </summary>
    const int MAX_BUFFER_STATES = 32;

    const float TIME_SYNC_DEADZONE = 0.25f;
    const float MIN_TIME_SCALE = 0.97f;
    const float MAX_TIME_SCALE = 1.03f;
    const float TELEPORT_THRESHOLD = 15.0f;

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