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
using System.Text;

namespace Clifton.TcpLib
{
	/// <summary>
	/// Implements a custom EventArgs class for passing connection state information.
	/// </summary>
	public class TcpServerEventArgs : EventArgs
	{
		protected ConnectionState connectionState;

		public ConnectionState ConnectionState
		{
			get { return connectionState; }
		}

		public TcpServerEventArgs(ConnectionState cs)
		{
			connectionState = cs;
		}
	}

	/// <summary>
	/// Implements a custom EventArgs class for passing the network stream connection
	/// information.
	/// </summary>
	public class TcpServiceEventArgs : EventArgs
	{
		protected NetworkStreamConnection connection;

		public NetworkStreamConnection Connection
		{
			get { return connection; }
		}

		public TcpServiceEventArgs(NetworkStreamConnection c)
		{
			connection = c;
		}
	}


	/// <summary>
	/// Implements a custom EventArgs class for passing an application exception back
	/// to the application for processing.
	/// </summary>
	public class TcpLibApplicationExceptionEventArgs : EventArgs
	{
		protected Exception e;

		public Exception Exception
		{
			get { return e; }
		}
		
		public TcpLibApplicationExceptionEventArgs(Exception e)
		{
			this.e = e;
		}
	}

}
