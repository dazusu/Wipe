using ProtoBuf;

namespace Wipe.Packets
{
    [ProtoContract]
    public abstract class Packet
    {
        [ProtoMember(1)]
        public ushort Id { get; set; }
    }
}
