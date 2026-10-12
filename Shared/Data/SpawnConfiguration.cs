using System;
using System.Collections.Generic;
using Godot;

namespace ROMA2.Shared.Data;

public class SpawnConfiguration
{
    private List<Vector3> _firstTeamPositions;
    private List<Vector3> _secondTeamPositions;

    public int Length => _firstTeamPositions.Count;

    public SpawnConfiguration(List<Vector3> fTeamPos, List<Vector3> sTeamPos)
    {
        if (fTeamPos.Count != sTeamPos.Count) 
            throw new InvalidOperationException("Длины списков позиций команд неодинаковые");

        _firstTeamPositions = fTeamPos;
        _secondTeamPositions = sTeamPos;
    } 

    public Vector3 GetSpawnPosition(TeamType team, int spawnPosId)
    {
        if (Length < spawnPosId) 
            throw new Exception("Индекс спавна больше, чем длина массива");
        
        List<Vector3> positions = team == TeamType.FirstTeam 
            ? _firstTeamPositions 
            : _secondTeamPositions;

        return positions[spawnPosId];
    }
}