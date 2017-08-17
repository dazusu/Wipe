using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Wipe.MMO.Network;
using SocketMessaging;
using Wipe.MMO.Entities;
using Wipe.MMO.Zones;

namespace Wipe.MMO
{
    public static class World
    {
        private static List<Zone> _worldZones;
        private static List<IEntity> _worldEntities;

        public static bool Start()
        {
            _worldEntities = new List<IEntity>();

            // Hook the server connection event.
            Server.Connection += Server_Connection;

            // Start the server.
            Server.Start();


            return true;
        }

        /// <summary>
        /// Handles client connections.
        /// </summary>
        /// <param name="connection"></param>
        private static void Server_Connection(Connection connection)
        {
            IEntity entity = new Player(connection);
        }
    }
}
