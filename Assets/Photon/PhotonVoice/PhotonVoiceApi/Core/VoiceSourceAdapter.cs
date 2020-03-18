﻿// -----------------------------------------------------------------------
// <copyright file="VoiceSourceAdapter.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
using System;
namespace Photon.Voice
{
    /// <summary>
    /// Adapter base class to move data by reading from <see cref="IDataReader.Read"></see> and pushing to <see cref="LocalVoice"></see>.
    /// </summary>
    /// <remarks>
    /// Use this with a LocalVoice of same T type.
    /// </remarks>
    public abstract class BufferReaderPushAdapterBase<T> : IServiceable
    {
        protected IDataReader<T> reader;
        /// <summary>Do the actual data read/push.</summary>
        /// <param name="localVoice">LocalVoice instance to push data to.</param>
        public abstract void Service(LocalVoice localVoice);
        /// <summary>Create a new BufferReaderPushAdapterBase instance</summary>
        /// <param name="reader">DataReader to read from.</param>
        public BufferReaderPushAdapterBase(IDataReader<T> reader)
        {
            this.reader = reader;
        }
        /// <summary>Release resources associated with this instance.</summary>
        public void Dispose()
        {
            this.reader.Dispose();
        }
    }
    /// <summary>
    /// Simple <see cref="BufferReaderPushAdapterBase"></see> implementation using a single buffer, using synchronous <see cref="LocalVoice.PushData"></see>
    /// </summary>
    public class BufferReaderPushAdapter<T> : BufferReaderPushAdapterBase<T>
    {
        protected T[] buffer;
        /// <summary>Create a new BufferReaderPushAdapter instance</summary>
        /// <param name="localVoice">LocalVoice instance to push data to.</param>
        /// <param name="reader">DataReader to read from.</param>
        public BufferReaderPushAdapter(LocalVoice localVoice, IDataReader<T> reader) : base(reader)
        {
            // any buffer will work but only of localVoice.FrameSize avoids additional processing
            buffer = new T[((LocalVoiceFramed<T>)localVoice).FrameSize];
        }
        public override void Service(LocalVoice localVoice)
        {
            while (this.reader.Read(this.buffer))
            {
                ((LocalVoiceFramed<T>)localVoice).PushData(this.buffer);
            }
        }
    }
    /// <summary>
    /// <see cref="BufferReaderPushAdapter"></see> implementation using asynchronous <see cref="LocalVoice.PushDataAsync"></see>.
    /// </summary>
    /// Acquires a buffer from pool before each Read, releases buffer after last Read (brings Acquire/Release overhead).
    /// 
    /// Expects localVoice to be a LocalVoiceFramed<T> of same T.
    public class BufferReaderPushAdapterAsyncPool<T> : BufferReaderPushAdapterBase<T>
    {
        /// <summary>Create a new BufferReaderPushAdapter instance</summary>
        /// <param name="localVoice">LocalVoice instance to push data to.</param>
        /// <param name="reader">DataReader to read from.</param>
        public BufferReaderPushAdapterAsyncPool(LocalVoice localVoice, IDataReader<T> reader) : base(reader) { }
        /// <summary>Do the actual data read/push.</summary>
        /// <param name="localVoice">LocalVoice instance to push data to. Must be a LocalVoiceFramed<T> of same T.</param>
        public override void Service(LocalVoice localVoice)
        {
            var v = ((LocalVoiceFramed<T>)localVoice);
            T[] buf = v.BufferFactory.New();
            while (this.reader.Read(buf))
            {
                v.PushDataAsync(buf);
                buf = v.BufferFactory.New();
            }
            // release unused buffer
            v.BufferFactory.Free(buf, buf.Length);
        }
    }
    /// <summary>
    /// <see cref="BufferReaderPushAdapter"></see> implementation using asynchronous <see cref="LocalVoice.PushDataAsync"></see> and data copy.
    /// </summary>
    /// Reads data to preallocated buffer, copies it to buffer from pool before pushing.
    /// Compared with <see cref="BufferReaderPushAdapterAsyncPool">, this avoids one pool Acquire/Release cycle at the cost
    /// of a buffer copy.
    /// 
    /// Expects localVoice to be a LocalVoiceFramed<T> of same T.
    public class BufferReaderPushAdapterAsyncPoolCopy<T> : BufferReaderPushAdapterBase<T>
    {
        protected T[] buffer;
        /// <summary>Create a new BufferReaderPushAdapter instance</summary>
        /// <param name="localVoice">LocalVoice instance to push data to.</param>
        /// <param name="reader">DataReader to read from.</param>
        public BufferReaderPushAdapterAsyncPoolCopy(LocalVoice localVoice, IDataReader<T> reader) : base(reader)
        {
            buffer = new T[((LocalVoiceFramedBase)localVoice).FrameSize];
        }
        /// <summary>Do the actual data read/push.</summary>
        /// <param name="localVoice">LocalVoice instance to push data to. Must be a LocalVoiceFramed<T> of same T.</param>
        public override void Service(LocalVoice localVoice)
        {
            while (this.reader.Read(buffer))
            {
                var v = ((LocalVoiceFramed<T>)localVoice);
                var buf = v.BufferFactory.New();
                Array.Copy(buffer, buf, buffer.Length);
                v.PushDataAsync(buf);
            }
        }
    }
    /// <summary>
    /// <see cref="BufferReaderPushAdapter"></see> implementation using asynchronous <see cref="LocalVoice.PushDataAsync"></see>, converting float samples to short.
    /// </summary>
    /// This adapter works exactly like <see cref="BufferReaderPushAdapterAsyncPool"></see>, but it converts float samples to short.
    /// Acquires a buffer from pool before each Read, releases buffer after last Read.
    /// 
    /// Expects localVoice to be a LocalVoiceFramed<T> of same T.
    public class BufferReaderPushAdapterAsyncPoolFloatToShort : BufferReaderPushAdapterBase<float>
    {
        float[] buffer;
        /// <summary>Create a new BufferReaderPushAdapter instance</summary>
        /// <param name="localVoice">LocalVoice instance to push data to.</param>
        /// <param name="reader">DataReader to read from.</param>
        public BufferReaderPushAdapterAsyncPoolFloatToShort(LocalVoice localVoice, IDataReader<float> reader) : base(reader)
        {
            buffer = new float[((LocalVoiceFramed<short>)localVoice).FrameSize];
        }
        /// <summary>Do the actual data read/push.</summary>
        /// <param name="localVoice">LocalVoice instance to push data to. Must be a LocalVoiceFramed<T> of same T.</param>
        public override void Service(LocalVoice localVoice)
        {
            var v = ((LocalVoiceFramed<short>)localVoice);
            short[] buf = v.BufferFactory.New();
            while (this.reader.Read(buffer))
            {
                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = (short)(buffer[i] * (float)short.MaxValue);
                }
                v.PushDataAsync(buf);
                buf = v.BufferFactory.New();
            }
            // release unused buffer
            v.BufferFactory.Free(buf, buf.Length);
        }
    }    
}
