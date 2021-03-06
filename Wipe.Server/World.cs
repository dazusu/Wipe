﻿using System;
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
using NLog;

namespace Wipe.MMO
{
    public static class World
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        private static readonly object _syncLock = new object();

        #region Project Reference Fields
        private static Fiber _fiber = new Fiber();
        #endregion

        #region World Fields
        private static List<IEntity> _players;
        private static List<Zone> _zones;
        private static int _entityCount = 0;
        #endregion

        #region Public Methods

        /// <summary>
        /// Starts this world.
        /// </summary>
        /// <returns></returns>
        public static bool Start()
        {

            _players = new List<IEntity>();

            // Hook the server connection event.
            Server.Connection += Server_Connection;

            // Start the server.
            Server.Start();

            return true;
        }

        public static int GetNextEntityId()
        {
            _entityCount++;
            return _entityCount;

        }

        #endregion

        #region Connection Handling.

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

        #endregion

        #region Private Methods

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

        #endregion
    }
}
