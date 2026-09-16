using Godot;

namespace ROMA2.Shared.Bootstrap;

public partial class Bootstrap : Node
{
    private const string _serverScenePath = "res://Server/Scenes/Main.tscn";
    private const string _clientScenePath = "res://Client/Scenes/Main.tscn";

    public override void _Ready()
    {
        string scenePath = IsServerMode() ? _serverScenePath : _clientScenePath;

        CallDeferred(nameof(ChangeScene), scenePath);
    }

    private static bool IsServerMode()
    {
        if (OS.HasFeature("dedicated_server")) return true;

        string[] args = OS.GetCmdlineArgs();
        foreach (string arg in args)
            if (arg == "--server" || arg == "-s") return true;

        return false;
    }

    private void ChangeScene(string scenePath)
    {
        Error error = GetTree().ChangeSceneToFile(scenePath);
        
        if (error != Error.Ok)
            GD.PrintErr($"[{nameof(Bootstrap)}] Не удалось загрузить сцену: {error}");
    }
}