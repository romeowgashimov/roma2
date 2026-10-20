using ROMA2.Shared.Configurations;

namespace ROMA2.Client.Logic.Services;

public static class CharactersConfigurationExtensions
{
    public static string GetCharacterVisuals(this CharactersConfiguration charsConfig, int id)
    {
        string path = "res://Client/Visuals/";
        path += "Characters/" + charsConfig.GetChampionName(id) + "Icon.png";

        return path;
    } 
}