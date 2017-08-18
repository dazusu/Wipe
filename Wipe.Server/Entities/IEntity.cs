using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipe.MMO.Zones;

namespace Wipe.MMO.Entities
{
    public interface IEntity
    {
        int EntityId { get; }
        bool IsConnected { get; }
        EntityLocation Location { get; }
        Zone CurrentZone { get; }


        void Dispose();
    }
}
