using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketMessaging;
using System.IO;
using System.Net;
using System.Runtime.Remoting.Messaging;
using ProtoBuf;
using Wipe.Packets;
using Wipe.MMO.Utilities;
using Wipe.MMO.Zones;

namespace Wipe.MMO.Entities
{
    public partial class Player
    {
        private Connection _connection;
        private MemoryStream _buffer = new MemoryStream();
        private long _bufferContinuePosition;
        private readonly ObjectRouter routeHandler = new ObjectRouter();


        private void InitializeRoutes()
        {
            routeHandler.SetRoute<CPKT_AuthRequest>(Handle_AuthRequest);
        }

        /// <summary>
        /// Handles recieving a packet from the player.
        /// </summary>
        /// <param name="sender">The "Connection" object, which is already referenced in this class.</param>
        private void _connection_ReceivedRaw(object sender, EventArgs e)
        {
            // Get the recieved bytes.
            byte[] received = _connection.Receive();

            // Write the bytes to our buffer for later processing.
            _buffer.Write(received, 0, received.Length);

            // Set the position of the buffer back to where we were upto previously.
            _buffer.Position = _bufferContinuePosition;

            try
            {
                while (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(_buffer, PrefixStyle.Base128, ProtoUtilities.GetPacketType, out object obj))
                {
                    _bufferContinuePosition = _buffer.Position;

                    Packet packet = (Packet)obj;

                    _fiber.Enqueue(() => HandlePacket(packet));

                }
            }
            catch (EndOfStreamException)
            {
                // partial packet. waiting for the rest to arrive.
            }

            if (_bufferContinuePosition == _buffer.Length)
            {
                _bufferContinuePosition = 0;
                _buffer.SetLength(0);
            }
        }

        private void HandlePacket(Packet packet)
        {
            bool handled = routeHandler.Route(packet);

            if (!handled)
            {
                
            }
            else
            {

            }
        }

        public void Send(Packet packet)
        {
            int? packetCode = ProtoUtilities.GetPacketTypeCode(packet.GetType());

            if (packetCode == null)
            {

            }
            else
            {
                if (_connection.IsConnected)
                {
                    byte[] data;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Serializer.NonGeneric.SerializeWithLengthPrefix(stream, packet, PrefixStyle.Base128, packetCode.Value);
                        data = stream.ToArray();
                        _connection.Send(data);
                    }
                }
            }
        }

        public void Respond(Packet packet, Packet response)
        {
            response.Id = packet.Id;
            Send(response);
        }

        private void Handle_AuthRequest(CPKT_AuthRequest authRequest)
        {
            IPEndPoint ipEndPoint = _connection.Socket.RemoteEndPoint as IPEndPoint;

            _log.Debug($"CPKT_AuthRequest from {ipEndPoint}");

            SPKT_AuthRequest response = new SPKT_AuthRequest() { EntityId = _entityId};

            // temp account management
            if (authRequest.Hash == "zaqwsxcdsxcderfvbgtyhnp")
            {
                _authenticated = true;
                _accountId = 9;
                _currentZone = new Zone();
                _location.Heading = 9;
                _location.X = 33;
                _location.Y = 12;
                _location.Zone = Area.UngurForest;
                _name = "Dazusu";

                _log.Debug($"{_name} logged in");

                response.Result = AuthResult.OK;
                response.X = _location.X;
                response.Y = _location.Y;
                response.Name = _name;
                response.ZoneID = (int) _location.Zone;
            }
            else
            {
                response.Result = AuthResult.BadLogin;
            }

            Respond(authRequest, response);
        }
    }
}
