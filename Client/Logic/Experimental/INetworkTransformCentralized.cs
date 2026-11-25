using ROMA2.Shared.Interfaces;

namespace ROMA2.Client.Logic.Experimental;

public interface INetworkTransformCentralized : INetworkTransform
{
    NetworkStateRingBuffer RingBuffer { get; }
}