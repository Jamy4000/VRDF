using POpusCodec.Enums;
using POpusCodec;
using System;
namespace Photon.Voice
{
    public class OpusCodec
    {
        public enum FrameDuration
        {
            Frame2dot5ms = 2500,
            Frame5ms = 5000,
            Frame10ms = 10000,
            Frame20ms = 20000,
            Frame40ms = 40000,
            Frame60ms = 60000
        }
        public static class Factory
        {
            static public IEncoder CreateEncoder<B>(VoiceInfo i, ILogger logger)
            {
                if (typeof(B) == typeof(float[]))
                    return new EncoderFloat(i, logger);
                else if (typeof(B) == typeof(short[]))
                    return new EncoderShort(i, logger);
                else
                    throw new UnsupportedCodecException("Factory.CreateEncoder<" + typeof(B) + ">", i.Codec, logger);
            }
        }
        public static class DecoderFactory
        {
            public static IEncoder Create<T>(VoiceInfo i, ILogger logger)
            {
                var x = new T[1];
                if (x[0].GetType() == typeof(float))
                    return new EncoderFloat(i, logger);
                else if (x[0].GetType() == typeof(short))
                    return new EncoderShort(i, logger);
                else
                    throw new UnsupportedCodecException("EncoderFactory.Create<" + x[0].GetType() + ">", i.Codec, logger);
            }
        }
        abstract public class Encoder<T> : IEncoderDirect<T[]>
        {        
            protected OpusEncoder encoder;
            protected bool disposed;
            protected Encoder(VoiceInfo i, ILogger logger)
            {
                try
                {
                    encoder = new OpusEncoder((SamplingRate)i.SamplingRate, (Channels)i.Channels, i.Bitrate, OpusApplicationType.Voip, (Delay)(i.FrameDurationUs * 2 / 1000));
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                    if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                    {
                        Error = "Exception in OpusCodec.Encoder constructor";
                    }
                    logger.LogError("[PV] OpusCodec.Encoder: " + Error);
                }
            }
            public string Error { get; private set; }
            public Action<ArraySegment<byte>> Output { set; get; }
            public void Input(T[] buf)
            {
                if (Error != null)
                {
                    return;
                }
                if (Output == null)
                {
                    Error = "OpusCodec.Encoder: Output action is not set";
                    return;
                }
                lock (this)
                {
                    if (disposed || Error != null) { }
                    else
                    {
                        var res = encodeTyped(buf);
                        if (res.Count != 0)
                        {
                            Output(res);
                        }
                    }
                }
            }
            private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });
            public ArraySegment<byte> DequeueOutput() { return EmptyBuffer; }
            protected abstract ArraySegment<byte> encodeTyped(T[] buf);
            public void Dispose()
            {
                lock (this)
                {
                    if (encoder != null)
                    {
                        encoder.Dispose();
                    }
                    disposed = true;
                }
            }
        }
        public class EncoderFloat : Encoder<float>
        {
            internal EncoderFloat(VoiceInfo i, ILogger logger) : base(i, logger) { }
            override protected ArraySegment<byte> encodeTyped(float[] buf)
            {
                return encoder.Encode(buf);
            }
        }
        public class EncoderShort : Encoder<short>
        {
            internal EncoderShort(VoiceInfo i, ILogger logger) : base(i, logger) { }
            override protected ArraySegment<byte> encodeTyped(short[] buf)
            {
                return encoder.Encode(buf);
            }
        }
        public abstract class Decoder<T> : IDecoder
        {
            protected OpusDecoder decoder;
            ILogger logger;
            public Decoder(Action<T[]> output, ILogger logger)
            {
                this.output = output;
                this.logger = logger;
            }
            public void Open(VoiceInfo i)
            {
                try
                {
                    decoder = new OpusDecoder((SamplingRate)i.SamplingRate, (Channels)i.Channels);
                }
                catch (Exception e)
                {
                    Error = e.ToString();
                    if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                    {
                        Error = "Exception in OpusCodec.Decoder constructor";
                    }
                    logger.LogError("[PV] OpusCodec.Decoder: " + Error);
                }
            }
            public string Error { get; private set; }
            private Action<T[]> output;
            public void Dispose()
            {
                if (decoder != null)
                {
                    decoder.Dispose();
                }
            }
            public void Input(byte[] buf)
            {
                if (Error == null)
                {
                    var res = decodeTyped(buf);
                    if (res.Length != 0)
                    {
                        output(res);
                    }
                }
            }
            protected abstract T[] decodeTyped(byte[] buf);
        }
        public class DecoderFloat : Decoder<float>
        {
            public DecoderFloat(Action<float[]> output, ILogger logger) : base(output, logger) { }
            override protected float[] decodeTyped(byte[] buf)
            {
                return decoder.DecodePacketFloat(buf);
            } 
        }
        public class DecoderShort : Decoder<short>
        {
            public DecoderShort(Action<short[]> output,ILogger logger) : base(output, logger) { }
            override protected short[] decodeTyped(byte[] buf)
            {
                return decoder.DecodePacketShort(buf);
            }
        }
        public class Util
        {
            internal static int bestEncoderSampleRate(int f)
            {
                int diff = int.MaxValue;
                int res = (int)SamplingRate.Sampling48000;
                foreach (var x in Enum.GetValues(typeof(SamplingRate)))
                {
                    var d = Math.Abs((int)x - f);
                    if (d < diff)
                    {
                        diff = d;
                        res = (int)x;
                    }
                }
                return res;
            }
        }
    }
}
