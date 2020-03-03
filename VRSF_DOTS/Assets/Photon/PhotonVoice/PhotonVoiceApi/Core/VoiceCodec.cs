﻿// -----------------------------------------------------------------------
// <copyright file="VoiceCodec.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
//#define PHOTON_VOICE_VIDEO_ENABLE
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Photon.Voice
{
    /// <summary>Generic encoder interface.</summary>
    /// Depending on implementation, encoder should either call Output on eaach data frame or return next data frame in DequeueOutput() call.
    public interface IEncoder : IDisposable
    {
        /// <summary>If not null, the object is in invalid state.</summary>
        string Error { get; }
        /// <summary>Set callback encoder calls on each encoded data frame (if such output supported).</summary>
        Action<ArraySegment<byte>> Output { set; get; }
        /// <summary>Returns next encoded data frame (if such output supported).</summary>
        ArraySegment<byte> DequeueOutput();
    }
    /// <summary>Interface for an encoder which consumes input data via explicit call.</summary>
    public interface IEncoderDirect<B> : IEncoder
    {
        /// <summary>Consumes the given raw data.</summary>
        /// <param name="buf">Array containing raw data (e.g. audio samples).</param>
        void Input(B buf);
    }
    /// <summary>Generic decoder interface.</summary>
    public interface IDecoder : IDisposable
    {
        /// <summary>Open (initialize) the decoder.</summary>
        /// <param name="info">Properties of the data stream to decode.</param>
        void Open(VoiceInfo info);
        /// <summary>If not null, the object is in invalid state.</summary>
        string Error { get; }
        /// <summary>Consumes the given encoded data.</summary>
        void Input(byte[] buf);
    }
    // Buffer for IEncoderDirect encoding images
    public struct ImageInputBuf
    {
        public IntPtr[] Buf;
        public int Width;
        public int Height;
        public int[] Stride;
        public ImageFormat ImageFormat;
        public Rotation Rotation;
        public Flip Flip;
    }
    // Buffer for output actions of image decoders
    public struct ImageOutputBuf
    {
        public IntPtr Buf;
        public int Width;
        public int Height;
        public int Stride;
    }
    public interface IDecoderQueuedOutputImageNative : IDecoder
    {
        ImageFormat OutputImageFormat { get; set; }
        Flip OutputImageFlip { get; set; }
        // if provided, decoder writes output to it 
        Func<int, int, IntPtr> OutputImageBufferGetter { get; set; }
    }
    /// <summary>Exception thrown if an unsupported audio sample type is encountered.</summary>
    /// <remarks>
    /// PhotonVoice generally supports 32-bit floating point ("float") or 16-bit signed integer ("short") audio,
    /// but it usually won't be converted automatically due to the high CPU overhead (and potential loss of precision) involved.
    /// </remarks>
    class UnsupportedSampleTypeException : Exception
    {
        /// <summary>Create a new UnsupportedSampleTypeException.</summary>
        /// <param name="t">The sample type actually encountered.</param>
        public UnsupportedSampleTypeException(Type t) : base("[PV] unsupported sample type: " + t) { }
    }
    /// <summary>Exception thrown if an unsupported codec is encountered.</summary>
    /// <remarks>PhotonVoice currently only supports one Codec, <see cref="Codec.AudioOpus"></see>.
    class UnsupportedCodecException : Exception
    {
        /// <summary>Create a new UnsupportedCodecException.</summary>
        /// <param name="info">The info prepending standard message.</param>
        /// <param name="codec">The codec actually encountered.</param>
        /// <param name="logger">Loogger.</param>
        public UnsupportedCodecException(string info, Codec codec, ILogger logger) : base("[PV] " + info + ": unsupported codec: " + codec) { }
    }
    /// <summary>Enum for Media Codecs supported by PhotonVoice.</summary>
    /// <remarks>Transmitted in <see cref="VoiceInfo"></see>. Do not change the values of this Enum!</remarks>
    public enum Codec
    {
		Raw = 1,
        /// <summary>OPUS audio</summary>
        AudioOpus = 11,
#if PHOTON_VOICE_VIDEO_ENABLE
        VideoVP8 = 21,
        VideoVP9 = 22,
        VideoH264 = 31,
#endif
    }
    public enum ImageFormat
    {
        Undefined,
        I420, // native vpx (no format conversion before encodong)                        
        YV12, // native vpx (no format conversion before encodong)
        Android420,
        RGBA,
        ABGR,
        BGRA,
        ARGB,
    }
    public enum Rotation
    {
        Undefined = -1,
        Rotate0 = 0,      // No rotation.
        Rotate90 = 90,    // Rotate 90 degrees clockwise.
        Rotate180 = 180,  // Rotate 180 degrees.
        Rotate270 = 270,  // Rotate 270 degrees clockwise.
    }
    public enum Flip
    {
        Undefined,
        None,
        Vertical,
        Horizontal
    }
    // Image buffer pool support
    public class ImageBufferInfo
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int[] Stride { get; private set; }
        public ImageFormat Format { get; private set; }
        public Rotation Rotation { get; set; }
        public Flip Flip { get; set; }
        public ImageBufferInfo(int width, int height, int[] stride, ImageFormat format)
        {
            Width = width;
            Height = height;
            Stride = stride;
            Format = format;
        }
    }
    public class ImageBufferNative
    {
        public ImageBufferNative(ImageBufferInfo info)
        {
            Info = info;
        }
        public ImageBufferInfo Info { get; protected set; }
        public IntPtr[] Planes { get; protected set; }
        // Release resources for dispose or reuse.
        public virtual void Release() { }
        public virtual void Dispose() { }
    }
    // Allocates native buffers for planes
    // Supports releasing to image pool with allocation reuse
    public class ImageBufferNativeAlloc : ImageBufferNative, IDisposable
    {
        ImageBufferNativePool<ImageBufferNativeAlloc> pool;
        public ImageBufferNativeAlloc(ImageBufferNativePool<ImageBufferNativeAlloc> pool, ImageBufferInfo info) : base(info)
        {
            this.pool = pool;
            Planes = new IntPtr[info.Stride.Length];
            for (int i = 0; i < info.Stride.Length; i++)
            {
                Planes[i] = System.Runtime.InteropServices.Marshal.AllocHGlobal(info.Stride[i] * info.Height);
            }
        }
        public override void Release()
        {
            if (pool != null)
            {
                pool.Release(this);
            }
        }
        public override void Dispose()
        {
            for (int i = 0; i < Info.Stride.Length; i++)
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(Planes[i]);
            }
        }
    }
    // Acquires byte[] plane via GHandle. Optimized for single plane images.
    // Supports releasing to image pool after freeing GHandle (object itself reused only)
    public class ImageBufferNativeGCHandleSinglePlane : ImageBufferNative, IDisposable
    {
        ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool;
        GCHandle planeHandle;
        public ImageBufferNativeGCHandleSinglePlane(ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool, ImageBufferInfo info) : base(info)
        {
            if (info.Stride.Length != 1)
            {
                throw new Exception("ImageBufferNativeGCHandleSinglePlane wrong plane count " + info.Stride.Length);
            }
            this.pool = pool;
            Planes = new IntPtr[1];
        }
        public void PinPlane(byte[] plane)
        {
            planeHandle = GCHandle.Alloc(plane, GCHandleType.Pinned);
            Planes[0] = planeHandle.AddrOfPinnedObject();
        }
        public override void Release()
        {
            planeHandle.Free();
            if (pool != null)
            {
                pool.Release(this);
            }
        }
        public override void Dispose()
        {
        }
    }
}
