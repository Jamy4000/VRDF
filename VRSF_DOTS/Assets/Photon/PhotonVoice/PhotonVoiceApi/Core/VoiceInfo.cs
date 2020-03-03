// -----------------------------------------------------------------------
// <copyright file="VoiceInfo.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
//#define PHOTON_VOICE_VIDEO_ENABLE
using System.Collections.Generic;
namespace Photon.Voice
{
	/// <summary>Describes stream properties.</summary>
	public struct VoiceInfo
    {
        /// <summary>
        /// Create stream info for an Opus audio stream.
        /// </summary>
        /// <param name="samplingRate">Audio sampling rate.</param>
        /// <param name="channels">Number of channels.</param>
        /// <param name="frameDurationUs">Uncompressed frame (audio packet) size in microseconds.</param>
        /// <param name="bitrate">Stream bitrate (in bits/second).</param>
        /// <param name="userdata">Optional user data. Should be serializable by Photon.</param>
        /// <returns>VoiceInfo instance.</returns>
        static public VoiceInfo CreateAudioOpus(POpusCodec.Enums.SamplingRate samplingRate, int channels, OpusCodec.FrameDuration frameDurationUs, int bitrate, object userdata = null)
        {
            return new VoiceInfo()
            {
                Codec = Codec.AudioOpus,
                SamplingRate = (int)samplingRate,
                Channels = channels,
                FrameDurationUs = (int)frameDurationUs,
                Bitrate = bitrate,
                UserData = userdata
            };
        }
		/// <summary>
		/// Create stream info for an Opus audio stream.
		/// </summary>
		/// <param name="samplingRate">Audio sampling rate.</param>
		/// <param name="channels">Number of channels.</param>
		/// <param name="frameDurationUs">Uncompressed frame (audio packet) size in microseconds.</param>
		/// <param name="bitrate">Stream bitrate (in bits/second).</param>
		/// <param name="userdata">Optional user data. Should be serializable by Photon.</param>
		/// <returns>VoiceInfo instance.</returns>
		static public VoiceInfo CreateAudio(Codec codec, int samplingRate, int channels, int frameDurationUs, object userdata = null)
		{
			return new VoiceInfo()
			{
				Codec = Codec.Raw,
				SamplingRate = (int)samplingRate,
				Channels = channels,
				FrameDurationUs = (int)frameDurationUs,
				UserData = userdata
			};
		}
#if PHOTON_VOICE_VIDEO_ENABLE
		/// <summary>
		/// Helper for VP8 stream info creation.
		/// </summary>
		/// <param name="bitrate">Stream bitrate.</param>
		/// <param name="width">Streamed video width. If 0, width and height of video source used (no rescaling).</param>
		/// <param name="heigth">Streamed video height. If -1, aspect ratio preserved during rescaling.</param>
		/// <param name="userdata">Optional user data. Should be serializable by Photon.</param>        
		/// <returns>VoiceInfo instance.</returns>
		static public VoiceInfo CreateVideoVP8(int bitrate, int width = 0, int heigth = -1, object userdata = null)
        {
            return new VoiceInfo()
            {
                Codec = Codec.VideoVP8,
                Bitrate = bitrate,
                Width = width,
                Height = heigth,
                UserData = userdata,
            };
        }
        /// <summary>
        /// Helper for VP9 stream info creation.
        /// </summary>
        /// <param name="bitrate">Stream bitrate.</param>
        /// <param name="width">Streamed video width. If 0, width and height of video source used (no rescaling).</param>
        /// <param name="heigth">Streamed video height. If -1, aspect ratio preserved during rescaling.</param>
        /// <param name="userdata">Optional user data. Should be serializable by Photon.</param>        
        /// <returns>VoiceInfo instance.</returns>
        static public VoiceInfo CreateVideo(Codec codec, int bitrate, int width = 0, int heigth = -1, object userdata = null)
        {
            return new VoiceInfo()
            {
                Codec = codec,
                Bitrate = bitrate,
                Width = width,
                Height = heigth,
                UserData = userdata,
            };
        }
#endif
        public override string ToString()
        {
            return "c=" + Codec + " f=" + SamplingRate + " ch=" + Channels + " d=" + FrameDurationUs + " s=" + FrameSize + " b=" + Bitrate + " w=" + Width + " h=" + Height + " ud=" + UserData;
        }
        public Codec Codec { get; set; }
        /// <summary>Audio sampling rate (frequency, in Hz).</summary>
        public int SamplingRate { get; set; }
        /// <summary>Number of channels.</summary>
        public int Channels { get; set; }
        /// <summary>Uncompressed frame (audio packet) size in microseconds.</summary>
        public int FrameDurationUs { get; set; }
        /// <summary>Target bitrate (in bits/second).</summary>
        public int Bitrate { get; set; }
        /// <summary>Optional user data. Should be serializable by Photon.</summary>
        public object UserData { get; set; }
        /// <summary>Uncompressed frame (data packet) size in samples.</summary>
        public int FrameDurationSamples { get { return (int)(this.SamplingRate * (long)this.FrameDurationUs / 1000000); } }
        /// <summary>Uncompressed frame (data packet) array size.</summary>
        public int FrameSize { get { return this.FrameDurationSamples * this.Channels; } }
        /// <summary>Video width (optional).</summary>
        public int Width { get; set; }
        /// <summary>Video height (optional)</summary>
        public int Height { get; set; }
    }
    /// <summary>Information about a remote voice (incoming stream).</summary>
    public class RemoteVoiceInfo
    {
        internal RemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo info)
        {
            this.ChannelId = channelId;
            this.PlayerId = playerId;
            this.VoiceId = voiceId;
            this.Info = info;
        }
        /// <summary>Remote voice info.</summary>
        public VoiceInfo Info { get; private set; }
        /// <summary>ID of channel used for transmission.</summary>
        public int ChannelId { get; private set; }
        /// <summary>Player ID of voice owner.</summary>
        public int PlayerId { get; private set; }
        /// <summary>Voice ID (unique in the room).</summary>
        public byte VoiceId { get; private set; }
    }
}
