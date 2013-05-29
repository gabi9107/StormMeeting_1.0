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
using System.Net;
using System.Net.Sockets;

namespace Clifton.TcpLib
{
	/// <summary>
	/// Provides a container for the network stream and the socket connection state,
	/// which includes the socket instance itself and the TcpServer instance.
	/// </summary>
	public class NetworkStreamConnection
	{
		protected NetworkStream networkStream;
		protected ConnectionState connectionState;

		/// <summary>
		/// Gets the ConnectionState instance managing the socket and TcpServer.
		/// </summary>
		public ConnectionState ConnectionState
		{
			get { return connectionState; }
		}

		/// <summary>
		/// Gets the network stream associated with the socket.
		/// </summary>
		public NetworkStream NetworkStream
		{
			get { return networkStream; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="ns">The network stream associated with the socket.</param>
		/// <param name="cs">The connection state instance associated with the socket.</param>
		public NetworkStreamConnection(NetworkStream ns, ConnectionState cs)
		{
			networkStream = ns;
			connectionState = cs;
		}

		/// <summary>
		/// Closes the network stream and underlying socket (by virtue of closing the network stream).
		/// </summary>
		public void Close()
		{
			networkStream.Close();
		}
	}
}
