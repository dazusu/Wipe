using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wipe.MMO.Utilities
{
    public class ObjectRouter
    {
        private Dictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        public void SetRoute<T>(Action<T> handler)
        {
            if (!_handlers.ContainsKey(typeof(T)))
            {
                _handlers.Add(typeof(T), null);
            }

            _handlers[typeof(T)] = (obj) => handler((T)obj);
        }

        public bool Route(object obj)
        {
            Action<object> handler = null;
            if (_handlers.TryGetValue(obj.GetType(), out handler))
            {
                handler(obj);
                return true;
            }

            return false;
        }
    }
}
