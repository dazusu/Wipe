using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketMessaging;
using System.IO;
using ProtoBuf;
using Wipe.Packets;

namespace Wipe.MMO.Entities
{
    class Player : IEntity
    {
        #region Fields
        private Connection _connection;
        private byte[] _receiveBuffer;
        private MemoryStream _buffer;
        private long _continue;
        #endregion


        /// <summary>
        /// Constructor for setting up a new player.
        /// </summary>
        /// <param name="connection"></param>
        public Player(Connection connection)
        {
            _receiveBuffer = new byte[] { };
            _buffer = new MemoryStream();

            _connection = connection;

            _connection.SetMode(MessageMode.Raw);

            _connection.ReceivedRaw += _connection_ReceivedRaw;
        }


        /// <summary>
        /// Handles recieving a packet from the player.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _connection_ReceivedRaw(object sender, EventArgs e)
        {
            byte[] received = _connection.Receive();

            _buffer.Write(received, 0, received.Length);
            _buffer.Position = _continue;

            object obj = default(object);

            try
            {
                long previous = _buffer.Position;

                while (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(_buffer, PrefixStyle.Base128, ProtoUtilities.GetPacketType, out obj))
                {
                    _continue = _buffer.Position;

                    Packet packet = (Packet)obj;

                    previous = _continue;
                }
            }
            catch(EndOfStreamException)
            {
                // partial packet. waiting for the rest to arrive.
            }

            if (_continue == _buffer.Length)
            {
                _continue = 0;
                _buffer.SetLength(0);
            }
        }
    }
}
