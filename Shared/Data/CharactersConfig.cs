namespace ROMA2.Shared.Data;

public class CharactersConfiguration
{
    private bool _isServer;
    private readonly string[] _charactersPaths =
    [
        "Aurora",
        "Sitaciya"
    ];

    public CharactersConfiguration(bool isServer)
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

    public string GetChampionName(int id)
    {
        return _charactersPaths[id];
    }
}