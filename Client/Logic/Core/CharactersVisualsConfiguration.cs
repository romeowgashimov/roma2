using ROMA2.Shared.Data;

namespace ROMA2.Client.Logic.Core;

public static class CharactersConfigurationExtensions
{
    public static string GetCharacterVisuals(this CharactersConfiguration charsConfig, int id)
    {
        string path = "res://Client/Visuals/";
        path += "Characters/" + charsConfig.GetChampionName(id) + "Icon.png";

        return path;
    } 
}