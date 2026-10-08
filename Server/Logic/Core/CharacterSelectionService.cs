using System;
using ROMA2.Shared;

namespace ROMA2.Server.Logic.Core;

public class CharacterSelectionService
{
    private ServerProperties _properties;
    private double _requiredTimeToSelect = 10;
    private double _currTime;
    private long _currPeer;
    private int _currPlayerId;
    private NetworkBridge _bridge;

    public event Action<GameState> ChangeState;

    public void Init(ServerProperties properties, NetworkBridge bridge)
    {
        _properties = properties;
        _bridge = bridge;
    }

    public void Run(double delta)
    {
        if (_properties.PlayersCount < _currPlayerId) ChangeState(GameState.Play);

        // Проверяем, если текущий пир игрока 0, значит никто ещё не назначен на выбор персонажа
        // Поэтому назначаем нового игрока для выбора
        if (_currPeer == 0)
        {
            _currPeer = _properties.GetPlayer(_currPlayerId);

            _bridge.Rpc(nameof(_bridge.OnSelectingCharacter), _currPeer);

            _currPlayerId++;
        }

        if (_currTime >= _requiredTimeToSelect)
        {
            _currTime = 0;
            _currPeer = 0;
            return;
        }

        _currTime += delta;
    }
}