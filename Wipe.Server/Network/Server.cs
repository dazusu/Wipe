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

namespace Wipe.MMO.Network
{
   internal static class Server
    {
        private static int _port = 52069;
        private static TcpServer _tcpServer;

        public delegate void ConnectionHandler(Connection connection);
        public static event ConnectionHandler Connection;

        public static void Start()
        {
            _tcpServer = new TcpServer();
            _tcpServer.Start(_port);
            _tcpServer.Connected += _tcpServer_Connected;
        }

        private static void _tcpServer_Connected(object sender, SocketMessaging.ConnectionEventArgs e)
        {
            Connection(e.Connection);
        }

    }
}
