using ROMA2.Shared.Configurations;

namespace ROMA2.Client.Logic.Extensions;

public static class CharactersConfigurationExtensions
{
    public static string GetCharacterVisuals(this PlayableCharacterConfiguration charsConfig, int id)
    {
        string path = "res://Client/Visuals/";
        path += "Characters/" + charsConfig.GetCharacterName(id) + "Icon.png";

        return path;
    } 
}