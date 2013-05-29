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

// Loosely based on the article http://www.codeproject.com/csharp/BasicTcpServer.asp

namespace Clifton.TcpLib
{
	/// <summary>
	/// Implements the core of a TcpServer socket listener.  This class makes no 
	/// assumptions regarding thread pool implementation, I/O interface such as
	/// streaming, command processing or connection management.
	/// Those details are left to the application.
	/// </summary>
	public class TcpServer
	{
		public delegate void TcpServerEventDlgt(object sender, TcpServerEventArgs e);
		public delegate void ApplicationExceptionDlgt(object sender, TcpLibApplicationExceptionEventArgs e);

		/// <summary>
		/// Event fires when a connection is accepted.  Being multicast, this allows
		/// you to attach not only your application's event handler, but also other handlers,
		/// such as diagnostics/monitoring, to the event.
		/// </summary>
		public event TcpServerEventDlgt Connected;

		/// <summary>
		/// This event fires when *your* application throws an exception that *you* do not handle in the 
		/// interaction with the client.  You can hook this event to log unhandled exceptions, more as a 
		/// tool to aid development rather than a suggested approach for handling your application errors.
		/// </summary>
		public event ApplicationExceptionDlgt HandleApplicationException;

		protected IPEndPoint endPoint;
		protected Socket listener;
		protected int pendingConnectionQueueSize;
		
		/// <summary>
		/// Gets/sets pendingConnectionQueueSize.  The default is 100.
		/// </summary>
		public int PendingConnectionQueueSize
		{
			get { return pendingConnectionQueueSize; }
			set
			{
				if (listener != null)
				{
					throw new TcpLibException("Listener has already started.  Changing the pending queue size is not allowed.");
				}

				pendingConnectionQueueSize = value; 
			}
		}
		
		/// <summary>
		/// Gets listener socket.
		/// </summary>
		public Socket Listener
		{
			get { return listener; }
		}
		
		/// <summary>
		/// Gets/sets endPoint
		/// </summary>
		public IPEndPoint EndPoint
		{
			get { return endPoint; }
			set
			{
				if (listener != null)
				{
					throw new TcpLibException("Listener has already started.  Changing the endpoint is not allowed.");
				}

				endPoint = value; 
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TcpServer()
		{
			pendingConnectionQueueSize = 100;
		}

		/// <summary>
		/// Initializes the server with an endpoint.
		/// </summary>
		/// <param name="endpoint"></param>
		public TcpServer(IPEndPoint endpoint)
		{
			this.endPoint = endpoint;
			pendingConnectionQueueSize = 100;
		}

		/// <summary>
		/// Initializes the server with a port, the endpoint is initialized
		/// with IPAddress.Any.
		/// </summary>
		/// <param name="port"></param>
		public TcpServer(int port)
		{
			endPoint = new IPEndPoint(IPAddress.Any, port);
			pendingConnectionQueueSize = 100;
		}

		/// <summary>
		/// Initializes the server with an IP4 or IP6 address and a port number.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="port"></param>
		public TcpServer(string address, int port)
		{
			endPoint = new IPEndPoint(IPAddress.Parse(address), port);
			pendingConnectionQueueSize = 100;
		}

		/// <summary>
		/// Begins listening for incoming connections.  This method returns immediately.
		/// Incoming connections are reported using the Connected event.
		/// </summary>
		public void StartListening()
		{
			if (endPoint == null)
			{
				throw new TcpLibException("EndPoint not initialized.");
			}

			if (listener != null)
			{
				throw new TcpLibException("Already listening.");
			}

			listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind(endPoint);
			listener.Listen(pendingConnectionQueueSize);
			listener.BeginAccept(AcceptConnection, null);
		}

		/// <summary>
		/// Shuts down the listener.
		/// </summary>
		public void StopListening()
		{
			// Make sure we're not accepting a connection.
			lock (this)
			{
				listener.Close();
				listener = null;
			}
		}

		/// <summary>
		/// Accepts the connection and invokes any Connected event handlers.
		/// </summary>
		/// <param name="res"></param>
		protected void AcceptConnection(IAsyncResult res)
		{
			Socket connection;

			// Make sure listener doesn't go null on us.
			lock (this)
			{
				connection = listener.EndAccept(res);
				listener.BeginAccept(AcceptConnection, null);
			}

			// Close the connection if there are no handlers to accept it!
			if (Connected == null)
			{
				connection.Close();
			}
			else
			{
				ConnectionState cs = new ConnectionState(connection, this);
				OnConnected(new TcpServerEventArgs(cs));
			}
		}

		/// <summary>
		/// Fire the Connected event if it exists.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnConnected(TcpServerEventArgs e)
		{
			if (Connected != null)
			{
				try
				{
					Connected(this, e);
				}
				catch (Exception ex)
				{
					// Close the connection if the application threw an exception that
					// is caught here by the server.
					e.ConnectionState.Close();
					TcpLibApplicationExceptionEventArgs appErr = new TcpLibApplicationExceptionEventArgs(ex);
					
					try
					{
						OnHandleApplicationException(appErr);
					}
					catch (Exception ex2)
					{
						// Oh great, the exception handler threw an exception!
						System.Diagnostics.Trace.WriteLine(ex2.Message);
					}
				}
			}
		}

		/// <summary>
		/// Invokes the HandleApplicationException handler, if exists.
		/// </summary>
		/// <param name="e">The exception event args instance.</param>
		protected virtual void OnHandleApplicationException(TcpLibApplicationExceptionEventArgs e)
		{
			if (HandleApplicationException != null)
			{
				HandleApplicationException(this, e);
			}
		}
	}
}
