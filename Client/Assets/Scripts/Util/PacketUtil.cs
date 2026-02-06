using System;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Protocol;

namespace Util
{
    public static class PacketUtil
    {
        public static PacketID GetPacketId(this IMessage packet)
        {
            string msgName = packet.Descriptor.Name;
            string pktEnumName = msgName.ToCleanCamelCase();
            
            if (Enum.TryParse<PacketID>(pktEnumName, true, out PacketID result))
            {
                return result;
            }
            return default;

            return PacketID.PktNone;

        }
    }
}