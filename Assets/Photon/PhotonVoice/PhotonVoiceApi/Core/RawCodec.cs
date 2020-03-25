using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace Photon.Voice
{
	public class RawCodec
	{
		public class Encoder<T> : IEncoderDirect<T[]>
		{
			public string Error { get; private set; }
			public Action<ArraySegment<byte>> Output { set; get; }
			private static readonly ArraySegment<byte> EmptyBuffer = new ArraySegment<byte>(new byte[] { });
			public ArraySegment<byte> DequeueOutput()
			{
				return EmptyBuffer;
			}
			public void Dispose()
			{				
			}			
			public void Input(T[] buf)
			{
				if (Error != null)
				{
					return;
				}
				if (Output == null)
				{
					Error = "RawCodec.Encoder: Output action is not set";
					return;
				}
				if (buf == null)
				{
					return;
				}
				if (buf.Length == 0)
				{
					return;
				}
				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream stream = new MemoryStream();
				bf.Serialize(stream, buf);
				Output(new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length));
			}
		}
		public class Decoder<T> : IDecoder
		{
			public string Error { get; private set; }
			public Decoder(Action<T[]> output)
			{
				this.output = output;
			}
			public void Open(VoiceInfo info)
			{
			}
			
			private Type outType = (new T[1])[0].GetType();
			
			public void Input(byte[] buf)
			{
				if (buf == null)
				{
					return;
				}
				if (buf.Length == 0)
				{
					return;
				}
				BinaryFormatter bf = new BinaryFormatter();
				MemoryStream stream = new MemoryStream(buf);
				var obj = bf.Deserialize(stream);
				if (obj.GetType() != outType)
				{
					var objFloat = obj as float[];
					if (objFloat != null)
					{
						var objShort = new short[objFloat.Length];
						AudioUtil.Convert(objFloat, objShort, objFloat.Length);
						output((T[])(object)objShort);
					}
					else
					{
						var objShort = obj as short[];
						if (objShort != null)
						{
							objFloat = new float[objShort.Length];
							AudioUtil.Convert(objShort, objFloat, objShort.Length);
							output((T[])(object)objFloat);
						}
					}
				}
				else
				{
					output((T[])obj);
				}
			}
			public void Dispose()
			{
			}
			private Action<T[]> output;
		}
	}
}
