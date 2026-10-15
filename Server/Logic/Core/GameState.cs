namespace ROMA2.Server.Logic.Core;

public enum GameState : byte
{
    WaitPlayers,
    WaitChoices,
    Initialization,
    Play,
    Pause,
    End,

    Restart
}