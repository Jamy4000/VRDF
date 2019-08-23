using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Photon.Voice
{
    // for convenience, it also calls VoiceClient payload handlers
    internal class PhotonTransportProtocol
    {
        enum EventSubcode : byte
        {
            VoiceInfo = 1,
            VoiceRemove = 2,
            Frame = 3,
        }

        enum EventParam : byte
        {
            VoiceId = 1,
            SamplingRate = 2,
            Channels = 3,
            FrameDurationUs = 4,
            Bitrate = 5,
            UserData = 10,
            EventNumber = 11,
            Codec = 12,
        }

        private VoiceClient voiceClient;
        private ILogger logger;

        public PhotonTransportProtocol(VoiceClient voiceClient, ILogger logger)
        {
            this.voiceClient = voiceClient;
            this.logger = logger;
        }

        internal object[] buildVoicesInfo(IEnumerable<LocalVoice> voicesToSend, bool logInfo)
        {
            object[] infos = new object[voicesToSend.Count()];

            object[] content = new object[] { (byte)0, EventSubcode.VoiceInfo, infos };
            int i = 0;
            foreach (var v in voicesToSend)
            {
                infos[i] = new Dictionary<byte, object>() {
                    { (byte)EventParam.VoiceId, v.id },
                    { (byte)EventParam.Codec, v.info.Codec },
                    { (byte)EventParam.SamplingRate, v.info.SamplingRate },
                    { (byte)EventParam.Channels, v.info.Channels },
                    { (byte)EventParam.FrameDurationUs, v.info.FrameDurationUs },
                    { (byte)EventParam.Bitrate, v.info.Bitrate },
                    { (byte)EventParam.UserData, v.info.UserData },
                    { (byte)EventParam.EventNumber, v.evNumber }

                };
                i++;

                if (logInfo)
                {
                    logger.LogInfo(v.LogPrefix + " Sending info: " + v.info.ToString() + " ev=" + v.evNumber);
                }
            }
            return content;
        }

        internal object[] buildVoiceRemoveMessage(LocalVoice v)
        {
            byte[] ids = new byte[] { v.id };

            object[] content = new object[] { (byte)0, EventSubcode.VoiceRemove, ids };

            logger.LogInfo(v.LogPrefix + " remove sent");

            return content;
        }

        internal object[] buildFrameMessage(byte voiceId, byte evNumber, ArraySegment<byte> data)
        {
            return new object[] { voiceId, evNumber, data };
        }

        internal void onVoiceEvent(object content0, int channelId, int playerId, int localPlayerId)
        {
            object[] content = (object[])content0;
            if ((byte)content[0] == (byte)0)
            {
                switch ((byte)content[1])
                {
                    case (byte)EventSubcode.VoiceInfo:
                        this.onVoiceInfo(channelId, playerId, content[2]);
                        break;
                    case (byte)EventSubcode.VoiceRemove:
                        this.onVoiceRemove(channelId, playerId, content[2]);
                        break;
                    default:
                        logger.LogError("[PV] Unknown sevent subcode " + content[1]);
                        break;
                }
            }
            else
            {
                byte voiceId = (byte)content[0];
                byte evNumber = (byte)content[1];
                byte[] receivedBytes = (byte[])content[2];
                this.voiceClient.onFrame(channelId, playerId, voiceId, evNumber, receivedBytes, playerId == localPlayerId);
            }
        }

        private void onVoiceInfo(int channelId, int playerId, object payload)
        {
            foreach (var el in (object[])payload)
            {
                var h = (Dictionary<byte, Object>)el;
                var voiceId = (byte)h[(byte)EventParam.VoiceId];
                var eventNumber = (byte)h[(byte)EventParam.EventNumber];
                var info = createVoiceInfoFromEventPayload(h);
                voiceClient.onVoiceInfo(channelId, playerId, voiceId, eventNumber, info);
            }
        }

        private void onVoiceRemove(int channelId, int playerId, object payload)
        {
            var voiceIds = (byte[])payload;
            voiceClient.onVoiceRemove(channelId, playerId, voiceIds);
        }

        private VoiceInfo createVoiceInfoFromEventPayload(Dictionary<byte, object> h)
        {
            var i = new VoiceInfo();
            i.SamplingRate = (int)h[(byte)EventParam.SamplingRate];
            i.Channels = (int)h[(byte)EventParam.Channels];
            i.FrameDurationUs = (int)h[(byte)EventParam.FrameDurationUs];
            i.Bitrate = (int)h[(byte)EventParam.Bitrate];
            i.UserData = h[(byte)EventParam.UserData];
            i.Codec = (Codec)h[(byte)EventParam.Codec];

            return i;
        }
    }
}
