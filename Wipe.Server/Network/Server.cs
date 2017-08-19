using SocketMessaging;
using SocketMessaging.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using Wipe.Packets;
using System.IO;
using NLog;
using System.Net;

namespace Wipe.MMO.Network
{
   internal static class Server
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #region Fields
        private static int _port = 52069;
        private static TcpServer _tcpServer;
        #endregion

        #region Events
        public delegate void ConnectionHandler(Connection connection);
        public static event ConnectionHandler Connection;
        #endregion

        public static void Start()
        {
            _tcpServer = new TcpServer();
            _tcpServer.Start(_port);
            _tcpServer.Connected += _tcpServer_Connected;
            _log.Info($"Server Initialized on {_port}");
        }

        private static void _tcpServer_Connected(object sender, ConnectionEventArgs e)
        {
            IPEndPoint ipEndPoint = e.Connection.Socket.RemoteEndPoint as IPEndPoint;

            _log.Info($"Accepted connection from {ipEndPoint.Address}");

            Connection(e.Connection);
        }

    }
}
