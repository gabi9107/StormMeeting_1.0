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
	/// Wraps the socket connection and TcpServer instance.
	/// </summary>
	public class ConnectionState
	{
		protected Socket connection;
		protected TcpServer server;
		
		/// <summary>
		/// Gets the TcpServer instance.  Throws an exception if the connection
		/// has been closed.
		/// </summary>
		public TcpServer Server
		{
			get
			{
				if (server == null)
				{
					throw new TcpLibException("Connection is closed.");
				}

				return server; 
			}
		}
		
		/// <summary>
		/// Gets the socket connection.  Throws an exception if the connection
		/// has been closed.
		/// </summary>
		public Socket Connection
		{
			get
			{
				if (server == null)
				{
					throw new TcpLibException("Connection is closed.");
				}

				return connection; 
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="connection">The socket connection.</param>
		/// <param name="server">The TcpServer instance.</param>
		public ConnectionState(Socket connection, TcpServer server)
		{
			this.connection = connection;
			this.server = server;
		}

		/// <summary>
		/// This is the prefered manner for closing a socket connection, as it
		/// nulls the internal fields so that subsequently referencing a closed
		/// connection throws an exception.  This method also throws an exception 
		/// if the connection has already been shut down.
		/// </summary>
		public void Close()
		{
			if (server == null)
			{
				throw new TcpLibException("Connection already is closed.");
			}

			connection.Shutdown(SocketShutdown.Both);
			connection.Close();
			connection = null;
			server = null;
		}
	}
}
