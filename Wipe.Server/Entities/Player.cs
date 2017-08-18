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
    public partial class Player : IEntity
    {
        #region Fields
        private bool _authenticated = false;
        private Fiber _fiber = new Fiber();
        private int _entityId;
        private EntityLocation _location;
        private Zone _currentZone;

        private EntityUpdate _entityUpdate = new EntityUpdate();
        private ZoneUpdate _zoneUpdate = new ZoneUpdate();
        #endregion

        #region Player Stats
        private int _velocity = 500;
        private string _name = "";
        #endregion

        #region properties
        public int EntityId => _entityId;
        public bool IsConnected => _connection.IsConnected;
        public EntityLocation Location => _location;
        public Zone CurrentZone => _currentZone;

        #endregion


        /// <summary>
        /// Constructor for setting up a new player.
        /// </summary>
        /// <param name="connection"></param>
        public Player(Connection connection)
        {
            _authenticated = true;
            _name = "Dazusu";
            _location = new EntityLocation()
            {
                X = 10,
                Y = 11,
                Heading = 9,
                Zone = Area.UngurForest
            };

            _currentZone = new Zone();

            _buffer = new MemoryStream();
            // Assign the player's connection reference.
            _connection = connection;
            // Set the connection mode to raw.
            _connection.SetMode(MessageMode.Raw);
            // Hook the ReceivedRaw event.
            _connection.ReceivedRaw += _connection_ReceivedRaw;

            _fiber.Enqueue(Update);
        }

        private void Update()
        {
            if (_authenticated)
            {
                _entityUpdate = new EntityUpdate()
                {
                    EntityId = _entityId,
                    Type = EntityType.PlayerCharacter,
                    Status = EntityStatus.Active,
                    X = _location.X,
                    Y = _location.Y,
                    Heading = _location.Heading,
                    Velocity = _velocity
                };

                if (CurrentZone != null)
                {
                    SendZoneUpdate();
                }
            }

            _fiber.Schedule(Update, TimeSpan.FromMilliseconds(Config.UPDATE_PLAYER_ENTITY_MS));
        }

        public void SendZoneUpdate()
        {

            _zoneUpdate.ServerTime = Environment.TickCount;

            List<EntityUpdate> updates = new List<EntityUpdate>();

            updates.Add(_entityUpdate);

            _zoneUpdate.EntityUpdates = updates;

            Send(_zoneUpdate);
        }




        public void Dispose()
        {
            _fiber.Enqueue(() =>
            {
                if (Location.Zone != Area.Undefined)
                {
                    CurrentZone.PlayerPart(this);
                }
            });
        }
    }
}
