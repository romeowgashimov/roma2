using MemoryPack;

namespace ROMA2.Shared.Data;

[MemoryPackable]
public partial struct ProjectileCreationData
{
    public int GlobalId;
    public int LocalId;
    public long OwnerPeer;
}