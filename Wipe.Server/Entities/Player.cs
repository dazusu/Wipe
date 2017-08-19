using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketMessaging;
using System.IO;
using NLog;
using ProtoBuf;
using Wipe.Packets;
using Wipe.MMO.Utilities;
using Wipe.MMO.Zones;

namespace Wipe.MMO.Entities
{
    public partial class Player : IEntity
    {
        private Logger _log = LogManager.GetCurrentClassLogger();
        private Fiber _fiber = new Fiber();

        #region Fields
        private bool _authenticated = false;

        private int _entityId;
        private int _accountId;
        private int _connectionId;

        private EntityLocation _location = new EntityLocation();
        private Zone _currentZone;
        private EntityUpdate _entityUpdate = new EntityUpdate();
        private ZoneUpdate _zoneUpdate = new ZoneUpdate();
        #endregion

        #region Player Stats
        private int _velocity = 0;
        private int _speed = 0;
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
            InitializeRoutes();

            // Assign connection information.
            _connection = connection;
            _connectionId = connection.Id;
            _entityId = World.GetNextEntityId();

            // Setup the connection to use raw messaging.
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
