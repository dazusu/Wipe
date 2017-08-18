using ProtoBuf;

namespace Wipe.Packets
{
    public enum AuthResult
    {
        Error = 0,
        OK = 1,
        BadLogin = 2,
        AlreadyLoggedIn = 3
    }

    /// <summary>
    /// Client Packet - AuthorizationRequest.
    /// </summary>
    [ProtoContract]
    public class CPKT_AuthRequest : Packet
    {
        [ProtoMember(1)]
        public string Hash { get; set; }
    }

    /// <summary>
    /// Server Packet - AuthorizationRequest Result.
    /// </summary>
    [ProtoContract]
    public class SPKT_AuthRequest : Packet
    {

        [ProtoMember(1)]
        public AuthResult Result;

        [ProtoMember(2)]
        public int EntityId;

        [ProtoMember(3)]
        public string Name;

        [ProtoMember(4)]
        public int ZoneID;

        [ProtoMember(5)]
        public int X;

        [ProtoMember(6)]
        public int Y;
    }
}
