using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wipe.Packets
{
    [ProtoContract]
    public class ZoneUpdate : Packet
    {
        [ProtoMember(1)]
        public int ServerTime;
        [ProtoMember(2)]
        public IEnumerable<EntityUpdate> EntityUpdates;
    }
}
