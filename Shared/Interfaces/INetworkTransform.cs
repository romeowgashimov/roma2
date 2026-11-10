using System.Collections.Generic;
using Godot;
using ROMA2.Shared.Data;

namespace ROMA2.Shared.Interfaces;

public interface INetworkTransform
{
    /// <summary>
    /// Задержка интерполяции в тиках (~50 мс при 60 Гц). При джиттере можно поднять до 4-6.
    /// </summary>
    const float INTERPOLATION_DELAY_TICKS = 3f;

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
    /// Допуск экстраполяции фактора t (0.25 = максимум на четверть сегмента вперёд).
    /// </summary>
    const float MAX_EXTRAPOLATION_SLACK = 0.25f;

    /// <summary>
    /// Страховка от разрастания буфера при бурстах пакетов.
    /// </summary>
    const int MAX_BUFFER_STATES = 32;

    uint ServerTick { get; set; }
    float VirtualRenderTick { get; set; }

    Vector3 NetworkPosition { get; set; }
    Quaternion NetworkQuaternion { get; set; }

    Vector3 Position { get; }
    Quaternion Quaternion { get; }
    Transform3D GlobalTransform { get; set; }

    List<NetworkState> StateBuffer { get; set; }
}
