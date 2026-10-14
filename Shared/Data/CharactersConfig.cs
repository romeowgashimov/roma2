namespace ROMA2.Shared.Data;

public class CharactersConfig
{
    private bool _isServer;
    private readonly string[] _charactersPaths =
    [
        "Aurora",
        "Sitaciya"
    ];

    public CharactersConfig(bool isServer)
    {
        _isServer = isServer;
    }

    public string GetChampionPath(int id)
    {
        string path = _isServer 
            ? "res://Server/Scenes/" 
            : "res://Client/Scenes/";
        path += $"{_charactersPaths[id]}.tscn";

        return path;
    }
}