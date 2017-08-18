using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketMessaging;
using System.IO;
using ProtoBuf;
using Wipe.Packets;
using Wipe.MMO.Utilities;
using Wipe.MMO.Zones;

namespace Wipe.MMO.Entities
{
    public partial class Player
    {
        private Connection _connection;
        private MemoryStream _buffer;
        private long _bufferContinuePosition;

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

            object obj = default(object);

            try
            {
                while (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(_buffer, PrefixStyle.Base128, ProtoUtilities.GetPacketType, out obj))
                {
                    _bufferContinuePosition = _buffer.Position;

                    Packet packet = (Packet)obj;

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
    }
}
