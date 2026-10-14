using System;
using Godot;
using Godot.Collections;
using ROMA2.Shared;

namespace ROMA2.Server.Logic.Core;

public class CharacterSelectionService
{
    private PlayerRepository _playerRepos;
    private double _requiredTimeToSelect = 10;
    private double _currTime;
    private long _currPeer;
    private int _currPlayerId;
    private NetworkBridge _bridge;

    public event Action<GameState> ChangeState;

    public void Init(PlayerRepository playerRepos, NetworkBridge bridge)
    {
        _playerRepos = playerRepos;
        _bridge = bridge;
        
        _bridge.CharacterSelected += OnCharacterSelected;
    }

    public void Run(double delta)
    {
        if (_playerRepos.PlayersCount <= _currPlayerId) 
            {
                ChangeState?.Invoke(GameState.Play);
                return;
            }

        // Проверяем, если текущий пир игрока 0, значит никто ещё не назначен на выбор персонажа
        // Поэтому назначаем нового игрока для выбора
        if (_currPeer == 0)
        {
            (_currPeer, _) = _playerRepos.GetPlayer(_currPlayerId);

            _bridge.Rpc(nameof(_bridge.OnSelectingCharacter), _currPeer);
        }

        if (_currTime >= _requiredTimeToSelect)
        {
            _currTime = 0;
            _currPeer = 0;
            return;
        }

        _currTime += delta;
    }

    public void OnCharacterSelected(long playerPeer, int characterId)
    {
        // Мы не проверяем порядок выбора, если первый выбирает, то выбрать может и второй
        if (_playerRepos.IsCharacterTaken(playerPeer, characterId))
        {
            _bridge.Rpc(nameof(_bridge.OnCharacterSelectionCanceled), playerPeer, true);
            return;
        }

        _playerRepos.AddCharacterId(playerPeer, characterId);
        //Когда мы выбрали персонажа, нужно дать выбор другому игроку
        _currTime = 0;
        _currPeer = 0;
        _currPlayerId++;

        _bridge.Rpc(nameof(_bridge.OnCharacterSelectionApproved), playerPeer, characterId);
    }
}