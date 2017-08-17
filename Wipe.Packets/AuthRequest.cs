using ProtoBuf;

namespace Wipe.Packets
{
    /// <summary>
    /// Client Packet - AuthorizationRequest.
    /// </summary>
    [ProtoContract]
    public class CPKT_AuthRequest : Packet
    {
        [ProtoMember(1)]
        public string AuthHash { get; set; }
    }

    /// <summary>
    /// Server Packet - AuthorizationRequest Result.
    /// </summary>
    [ProtoContract]
    public class SPKT_AuthRequest : Packet
    {
        public enum AuthResult
        {
            Error = 0,
            OK = 1,
            BadLogin = 2,
            AlreadyLoggedIn = 3
        }

        [ProtoMember(1)]
        public AuthResult Result { get; set; }

        [ProtoMember(2)]
        public int EntityId { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }

        [ProtoMember(4)]
        public int ZoneID { get; set; }

        [ProtoMember(5)]
        public int X { get; set; }

        [ProtoMember(6)]
        public int Y { get; set; }
    }
}
