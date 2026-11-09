namespace ROMA2.Shared.Configurations;

public class PlayableCharacterConfiguration
{
    private bool _isServer;
    private readonly string[] _charactersPaths =
    [
        "Aurora",
        "Sitaciya"
    ];
    private int _characterCount;

    public int CharactersCount
    {
        get
        {
            _characterCount++;
            return _characterCount;
        }
        private set
        {
            _characterCount = value;
        }
    }

    public PlayableCharacterConfiguration(bool isServer)
    {
        _isServer = isServer;
    }

    public string GetCharacterPath(int id)
    {
        string path = _isServer 
            ? "res://Server/Scenes/" 
            : "res://Client/Scenes/";
        path += $"{_charactersPaths[id]}.tscn";

        return path;
    }

    public string GetCharacterName(int id)
    {
        return _charactersPaths[id];
    }
}