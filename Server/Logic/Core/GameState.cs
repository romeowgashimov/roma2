namespace ROMA2.Server.Logic.Core;

public enum GameState : byte
{
    WaitPlayers,
    WaitChoices,
    Play,
    Pause,
    End,

    Restart
}