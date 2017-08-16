using System;
using System.Runtime.InteropServices;

namespace Wipe.Packets
{
    /// <summary>
    /// Serializes and Deserializes Structs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Serializer<T>
    {
        public T Deserialize(byte[] raw)
        {
            return Deserialize(raw, 0);
        }

        public T Deserialize(byte[] raw, int position)
        {
            int size = Marshal.SizeOf<T>();

            if (size > raw.Length)
            {
                return default(T);
            }

            IntPtr buffer = Marshal.AllocHGlobal(size);

            Marshal.Copy(raw, position, buffer, size);

            T obj = (T)Marshal.PtrToStructure<T>(buffer);

            Marshal.FreeHGlobal(buffer);

            return obj;
        }

        public byte[] Serialize(T item)
        {
            int size = Marshal.SizeOf<T>();

            IntPtr buffer = Marshal.AllocHGlobal(size);

            Marshal.StructureToPtr<T>(item, buffer, false);

            byte[] raw = new byte[size];

            Marshal.Copy(buffer, raw, 0, size);

            Marshal.FreeHGlobal(buffer);

            return raw;
        }
    }
}
