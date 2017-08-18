using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Wipe.Packets
{
    public enum EntityType
    {
        Unassigned,
        PlayerCharacter,
        NonPlayerCharacter,
        Enemy,
        Object,
        Pet
    }

    public enum EntityStatus
    {
        Unassigned,
        Loading,
        Active,
        Inactive
    }

    [ProtoContract]
    public class EntityUpdate : Packet
    {
        [ProtoMember(2)]
        public int EntityId;
        [ProtoMember(3)]
        public EntityType Type;
        [ProtoMember(4)]
        public EntityStatus Status;
        [ProtoMember(5)]
        public int X;
        [ProtoMember(6)]
        public int Y;
        [ProtoMember(7)]
        public int Heading;
        [ProtoMember(8)]
        public int Velocity;
    }
}
