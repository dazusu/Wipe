using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipe.MMO.Entities;

namespace Wipe.MMO.Zones
{
    public class Zone
    {
        private object _syncLock = new object();
        private List<IEntity> _players;

        public Zone()
        {

        }

        public void PlayerJoin(IEntity entity)
        {
            lock(_syncLock)
            {
                _players.Add(entity);
            }
        }

        public void PlayerPart(IEntity entity)
        {
            lock(_syncLock)
            {
                _players.Remove(entity);
            }
        }
    }
}
