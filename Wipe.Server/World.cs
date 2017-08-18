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
using Wipe.MMO.Utilities;
using System.Collections.Concurrent;

namespace Wipe.MMO
{
    public static class World
    {
        private static object _syncLock = new object();
        private static List<IEntity> _players;
        private static List<Zone> _zones;
        private static Fiber _fiber = new Fiber();

        public static bool Start()
        {
            _players = new List<IEntity>();

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

            lock(_syncLock)
            {
                _players.Add(entity);
            }
        }

        /// <summary>
        /// Checks for disconnected clients.
        /// </summary>
        private static void UpdateWorld()
        {
            List<IEntity> players = new List<IEntity>(_players);

            Parallel.ForEach(players, player =>
            {
                if (!player.IsConnected)
                {
                    player.Dispose();
                }
            });
        }
    }
}
