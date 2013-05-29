/*
Copyright (c) 2006, Marc Clifton
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list
  of conditions and the following disclaimer. 

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other
  materials provided with the distribution. 
 
* Neither the name Marc Clifton nor the names of contributors may be
  used to endorse or promote products derived from this software without specific
  prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Clifton.Tools.Data;

// Something to read: http://www.hightechtalks.com/clr/cryptostream-dispose-closes-target-stream-148724.html
// Block cypher and the read problem: http://bugs.ximian.com/long_list.cgi?buglist=60573
// More on the CryptoStream/NetworkStream bug: http://www.cs.cornell.edu/courses/cs513/2004sp/04.announceArchive.html
// A NetworkStream example from Microsoft: http://windowssdk.msdn.microsoft.com/library/default.asp?url=/library/en-us/dv_fxsecurity/html/9b266b6c-a9b2-4d20-afd8-b3a0d8fd48a0.asp
// The reason! http://www.derkeiler.com/Newsgroups/microsoft.public.dotnet.security/2004-02/0420.html
// CBC stream cipher in C#: http://www.codeproject.com/dotnet/csstreamcipher.asp
// CipherMode enumeration: ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref11/html/T_System_Security_Cryptography_CipherMode.htm

namespace Clifton.TcpLib
{
	/// <summary>
	/// Raw, compressed, secure connection service.  This service double-buffers the
	/// serialization stream.  This is necessary to keep the network stream open
	/// after the GZip and Crypto streams are closed (the only way to flush their data).
	/// As such, the service requires that the application work with packets of data,
	/// specifying begin read/write and end read/write operations to clearly delineate
	/// the packet.
	/// </summary>
	public class RCSConnectionService
	{
		protected NetworkStreamConnection connection;
		protected NetworkStream networkStream;
		protected RawSerializer serializer;
		protected RawDeserializer deserializer;
		protected byte[] key;
		protected byte[] iv;
		protected CryptoStream encStream;
		protected GZipStream comp;
		protected CryptoStream decStream;
		protected GZipStream decomp;
		protected MemoryStream writeBuffer;

		/// <summary>
		/// Returns the network stream connection instance.
		/// </summary>
		public NetworkStreamConnection Connection
		{
			get { return connection; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connection">The network stream connection instance.</param>
		/// <param name="key">The crypto key.</param>
		/// <param name="iv">The crypto initial vector.</param>
		public RCSConnectionService(NetworkStreamConnection connection, byte[] key, byte[] iv)
		{
			this.key = key;
			this.iv = iv;
			this.connection = connection;
			networkStream = connection.NetworkStream;
		}

		/// <summary>
		/// Blocking read.  Initialize a crypto and GZip stream from the packet sitting
		/// in the NetworkStream.
		/// </summary>
		public void BeginRead()
		{
			try
			{
				byte[] plength = new byte[sizeof(Int32)];							// Get packet length length.
				networkStream.Read(plength, 0, plength.Length);						// Read the length.
				int l = BitConverter.ToInt32(plength, 0);							// Convert to an Int32
				byte[] buffer = new byte[l];										// Initialize the buffer.
				networkStream.Read(buffer, 0, l);									// Read the packet data.
				MemoryStream stream = new MemoryStream(buffer);						// Initialize the memory stream.
				InitializeDeserializer(stream, key, iv);							// Initialize the deserialization stack.
			}
			catch (Exception e)
			{
				throw new TcpLibException(e.Message);
			}
		}

		/// <summary>
		/// Closes the crypto and GZip streams.  Because an intermediate memory stream is used,
		/// the NetworkStream is left open.
		/// </summary>
		public void EndRead()
		{
			try
			{
				decomp.Close();														// Close GZip stream.
				decStream.Close();													// Close crypto stream.
			}
			catch (Exception e)
			{
				throw new TcpLibException(e.Message);
			}
		}

		/// <summary>
		/// Begin writing by initializing a MemoryStream as the output buffer for the zipped, encrypted data.
		/// </summary>
		public void BeginWrite()
		{
			try
			{
				InitializeSerializer(key, iv);
			}
			catch (Exception e)
			{
				throw new TcpLibException(e.Message);
			}
		}

		/// <summary>
		/// Write the buffered packet of data to the network stream.
		/// </summary>
		public void EndWrite()
		{
			try
			{
				comp.Close();														// Close the GZip stream, flushing the final block.
				encStream.FlushFinalBlock();										// Flush the final block of the crypto stream without closing.
				byte[] length = BitConverter.GetBytes((int)writeBuffer.Length);		// Get the length as a byte[]
				networkStream.Write(length, 0, length.Length);					// Write the length.
				networkStream.Write(writeBuffer.GetBuffer(), 0, (int)writeBuffer.Length);	// Write the buffer.
				encStream.Close();												// Close the crypto stream.
			}
			catch (Exception e)
			{
				throw new TcpLibException(e.Message);
			}
		}

		/// <summary>
		/// Read an object of Type t from the network stream's packet.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public object Read(Type t)
		{
			object obj = deserializer.Deserialize(t);
			return obj;
		}

		/// <summary>
		/// Read a string from the network stream's packet.
		/// </summary>
		/// <returns></returns>
		public string ReadString()
		{
			string str = deserializer.DeserializeString();
			return str;
		}

		/// <summary>
		/// Read a DataTable from the network stream's packet.
		/// </summary>
		/// <returns></returns>
		public DataTable ReadDataTable()
		{
			DataTable dt = RawDataTable.Deserialize(deserializer);
			return dt;
		}

		/// <summary>
		/// Returns a byte[] from the stream.
		/// </summary>
		/// <returns>A byte array.</returns>
		public byte[] ReadByteArray()
		{
			return deserializer.DeserializeBytes();
		}

		/// <summary>
		/// Returns a Guid from the stream.
		/// </summary>
		/// <returns>A Guid.</returns>
		public Guid ReadGuid()
		{
			return deserializer.DeserializeGuid();
		}

		/// <summary>
		/// Returns an Int32 from the stream.
		/// </summary>
		/// <returns>An Int32.</returns>
		public int ReadInt32()
		{
			return deserializer.DeserializeInt();
		}

		/// <summary>
		/// Write an arbitrary object (value type or marshallable struct) to the network stream packet.
		/// </summary>
		/// <param name="obj"></param>
		public void Write(object obj)
		{
			serializer.Serialize(obj);
		}

		/// <summary>
		/// Write a bool to the network stream packet.
		/// </summary>
		/// <param name="b"></param>
		public void Write(bool b)
		{
			serializer.Serialize(b);
		}

		/// <summary>
		/// Write a string to the network stream's packet.
		/// </summary>
		/// <param name="str"></param>
		public void Write(string str)
		{
			serializer.Serialize(str);
		}

		/// <summary>
		/// Write a DataTable to the network stream's packet.
		/// </summary>
		/// <param name="dt"></param>
		public void Write(DataTable dt)
		{
			RawDataTable.Serialize(serializer, dt);
		}

		/// <summary>
		/// Writes a byte array to the stream.
		/// </summary>
		/// <param name="data">A byte[].</param>
		public void Write(byte[] data)
		{
			serializer.Serialize(data);
		}

		/// <summary>
		/// Writes a Guid to the stream.
		/// </summary>
		/// <param name="guid">A Guid.</param>
		public void Write(Guid guid)
		{
			serializer.Serialize(guid);
		}

		/// <summary>
		/// Writes an Int32 to the stream.
		/// </summary>
		/// <param name="i">An Int32.</param>
		public void Write(int i)
		{
			serializer.Serialize(i);
		}

		/// <summary>
		/// Initialize the serializer, consisting of the following stack:
		/// GZip -> Crypto -> MemoryStream
		/// </summary>
		/// <param name="key"></param>
		/// <param name="iv"></param>
		protected void InitializeSerializer(byte[] key, byte[] iv)
		{
			writeBuffer = new MemoryStream();
			EncryptTransformer et = new EncryptTransformer(EncryptionAlgorithm.Rijndael);
			et.IV = iv;
			ICryptoTransform ict = et.GetCryptoServiceProvider(key);
			encStream = new CryptoStream(writeBuffer, ict, CryptoStreamMode.Write);
			comp = new GZipStream(encStream, CompressionMode.Compress, true);		// important to be set to true!
			serializer = new RawSerializer(comp);
		}

		/// <summary>
		/// Initialize the deserializer, consisting of the following stack:
		/// MemoryStream -> Crypto -> GZip
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="key"></param>
		/// <param name="iv"></param>
		protected void InitializeDeserializer(Stream stream, byte[] key, byte[] iv)
		{
			DecryptTransformer dt = new DecryptTransformer(EncryptionAlgorithm.Rijndael);
			dt.IV = iv;
			ICryptoTransform ict = dt.GetCryptoServiceProvider(key);
			decStream = new CryptoStream(stream, ict, CryptoStreamMode.Read);
			decomp = new GZipStream(decStream, CompressionMode.Decompress);
			deserializer = new RawDeserializer(decomp);
		}
	}
}
