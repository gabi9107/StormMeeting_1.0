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
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Clifton.TcpLib;
using Clifton.Threading;

namespace Clifton.TcpLib
{
	/// <summary>
	/// Implements a managed thread pool wrapper for the connection.
	/// On connection, a worker thread is created which fires the Connected
	/// event.  Stephen Toub's managed thread pool is used for the the thread pool
	/// implementation, so that the worker thread is not constrained in time, which
	/// causes problems with the .NET thread pool.
	/// </summary>
	public class PooledStreamTcpService
	{
		public delegate void ConnectedDlgt(object sender, TcpServiceEventArgs e);
		public delegate void ApplicationExceptionDlgt(object sender, TcpLibApplicationExceptionEventArgs e);

		/// <summary>
		/// This event fires when a client is connected.  Hook this event to begin interacting with the client.
		/// </summary>
		public event ConnectedDlgt Connected;

		/// <summary>
		/// This event fires when *your* application throws an exception that *you* do not handle in the 
		/// interaction with the client.  You can hook this event to log unhandled exceptions, more as a 
		/// tool to aid development rather than a suggested approach for handling your application errors.
		/// </summary>
		public event ApplicationExceptionDlgt HandleApplicationException;

		protected TcpServer tcpLib;
		protected int readTimeout;
		protected int writeTimeout;
		
		/// <summary>
		/// Gets/sets writeTimeout
		/// </summary>
		public int WriteTimeout
		{
			get { return writeTimeout; }
			set { writeTimeout = value; }
		}
		
		/// <summary>
		/// Gets/sets readTimeout
		/// </summary>
		public int ReadTimeout
		{
			get { return readTimeout; }
			set { readTimeout = value; }
		}
		
		/// <summary>
		/// Returns the TcpServer instance.
		/// </summary>
		public TcpServer TcpLib
		{
			get { return tcpLib; }
		}

		/// <summary>
		/// Constructor.  The port on which to listen.  Will use IPAddress.Any for the address.
		/// </summary>
		/// <param name="port"></param>
		public PooledStreamTcpService(int port)
		{
			readTimeout = Timeout.Infinite;
			writeTimeout = Timeout.Infinite;
			tcpLib = new TcpServer(port);
			tcpLib.Connected += new TcpServer.TcpServerEventDlgt(OnConnected);
		}

		/// <summary>
		/// Constructor.  The IP address and port on which to listen.
		/// </summary>
		/// <param name="address"></param>
		/// <param name="port"></param>
		public PooledStreamTcpService(string address, int port)
		{
			readTimeout = Timeout.Infinite;
			writeTimeout = Timeout.Infinite;
			tcpLib = new TcpServer(address, port);
			tcpLib.Connected += new TcpServer.TcpServerEventDlgt(OnConnected);
		}

		/// <summary>
		/// Starts listening for connections.
		/// </summary>
		public void Start()
		{
			tcpLib.StartListening();
		}

		/// <summary>
		/// Stops listening for connections.
		/// </summary>
		public void Stop()
		{
			tcpLib.StopListening();
		}

		/// <summary>
		/// When a connection is made, get a network stream and start the worker thread.
		/// The worker thread uses the managed thread pool, which is safe for handling
		/// work that takes a lot of time. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnConnected(object sender, TcpServerEventArgs e)
		{
			ConnectionState cs = e.ConnectionState;
			NetworkStream ns = new NetworkStream(cs.Connection, true);
			ns.ReadTimeout = readTimeout;
			ns.WriteTimeout = writeTimeout;
			NetworkStreamConnection conn = new NetworkStreamConnection(ns, cs);
			ManagedThreadPool.QueueUserWorkItem(ConnectionProcess, conn);
		}

		/// <summary>
		/// Inform the application that we are connected.  This event fires in a worker thread.
		/// </summary>
		/// <param name="state"></param>
		protected void ConnectionProcess(object state)
		{
			NetworkStreamConnection conn = (NetworkStreamConnection)state;

			try
			{
				OnConnected(new TcpServiceEventArgs(conn));
			}
			catch (Exception e)
			{
				// Report exception not caught by the application.
				try
				{
					OnHandleApplicationException(new TcpLibApplicationExceptionEventArgs(e));
				}
				catch (Exception ex2)
				{
					// Oh great the app's handler threw an exception!
					System.Diagnostics.Trace.WriteLine(ex2.Message);
				}
				finally
				{
					// In any case, close the connection.
					conn.Close();
				}
			}
		}

		/// <summary>
		/// Invokes the Connected handler, if exists.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnConnected(TcpServiceEventArgs e)
		{
			if (Connected != null)
			{
				Connected(this, e);
			}
		}

		/// <summary>
		/// Invokes the HandleApplicationException handler, if exists.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnHandleApplicationException(TcpLibApplicationExceptionEventArgs e)
		{
			if (HandleApplicationException != null)
			{
				HandleApplicationException(this, e);
			}
		}
	}
}
