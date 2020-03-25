namespace Photon.Voice.Unity
{
    using Voice;
    using System;

    public class RemoteVoiceLink
    {
        public VoiceInfo Info { get; private set; }
        public int PlayerId { get; private set; }
        public int VoiceId { get; private set; }
        public int ChannelId { get; private set; }

        public event Action<float[]> FloatFrameDecoded;
        public event Action RemoteVoiceRemoved;

        public RemoteVoiceLink(VoiceInfo info, int playerId, int voiceId, int channelId,
            ref RemoteVoiceOptions options)
        {
            this.Info = info;
            this.PlayerId = playerId;
            this.VoiceId = voiceId;
            this.ChannelId = channelId;
            options.SetOutput(OnDecodedFrameFloatAction);
            options.OnRemoteVoiceRemoveAction = OnRemoteVoiceRemoveAction;
        }

        private void OnRemoteVoiceRemoveAction()
        {
            if (this.RemoteVoiceRemoved != null)
            {
                this.RemoteVoiceRemoved();
            }
        }

        private void OnDecodedFrameFloatAction(float[] floats)
        {
            if (this.FloatFrameDecoded != null)
            {
                this.FloatFrameDecoded(floats);
            }
        }
    }
}
