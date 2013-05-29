/*
Copyright (c) 2005, Marc Clifton
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list
  of conditions and the following disclaimer. 

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other
  materials provided with the distribution. 
 
* Neither the name of Marc Clifton nor the names of contributors may be
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

// Source: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/secmod/html/secmod24.asp

// DES has 8 byte key and IV
// Rijndael has 16 byte key and 16 byte IV
// 3DES can have 16 or 24 byte key and 8 byte IV ???

using System;
using System.Security.Cryptography;

namespace Clifton.Tools.Data
{
	public enum EncryptionAlgorithm
	{
		Des=0,
		Rc2,
		Rijndael,
		TripleDes,
		RC4,
	}

	public class EncryptTransformer
	{
		private EncryptionAlgorithm algorithmID;
		private byte[] initVec;
		private byte[] encKey;

		public byte[] IV
		{
			get {return initVec;}
			set {initVec = value;}
		}

		public byte[] Key
		{
			get {return encKey;}
		}

		public EncryptTransformer(EncryptionAlgorithm algId)
		{
			//Save the algorithm being used.
			algorithmID = algId;
			initVec=null;
			encKey=null;
		}

		public ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
		{
			// Pick the provider.
			switch (algorithmID)
			{
				case EncryptionAlgorithm.Des:
				{
					DES des = new DESCryptoServiceProvider();
					des.Mode = CipherMode.CBC;

					// See if a key was provided
					if (null == bytesKey)
					{
						encKey = des.Key;
					}
					else
					{
						des.Key = bytesKey;
						encKey = des.Key;
					}
					// See if the client provided an initialization vector
					if (null == initVec)
					{ // Have the algorithm create one
						initVec = des.IV;
					}
					else
					{ //No, give it to the algorithm
						des.IV = initVec;
					}
					return des.CreateEncryptor();
				}
				case EncryptionAlgorithm.TripleDes:
				{
					TripleDES des3 = new TripleDESCryptoServiceProvider();
					des3.Mode = CipherMode.CBC;
					// See if a key was provided
					if (null == bytesKey)
					{
						encKey = des3.Key;
					}
					else
					{
						des3.Key = bytesKey;
						encKey = des3.Key;
					}
					// See if the client provided an IV
					if (null == initVec)
					{ //Yes, have the alg create one
						initVec = des3.IV;
					}
					else
					{ //No, give it to the alg.
						des3.IV = initVec;
					}
					return des3.CreateEncryptor();
				}
				case EncryptionAlgorithm.Rc2:
				{
					RC2 rc2 = new RC2CryptoServiceProvider();
					rc2.Mode = CipherMode.CBC;
					// Test to see if a key was provided
					if (null == bytesKey)
					{
						encKey = rc2.Key;
					}
					else
					{
						rc2.Key = bytesKey;
						encKey = rc2.Key;
					}
					// See if the client provided an IV
					if (null == initVec)
					{ //Yes, have the alg create one
						initVec = rc2.IV;
					}
					else
					{ //No, give it to the alg.
						rc2.IV = initVec;
					}
					return rc2.CreateEncryptor();
				}
				case EncryptionAlgorithm.Rijndael:
				{
					Rijndael rijndael = new RijndaelManaged();
					rijndael.Mode = CipherMode.CBC;
					// Test to see if a key was provided
					if(null == bytesKey)
					{
						encKey = rijndael.Key;
					}
					else
					{
						rijndael.Key = bytesKey;
						encKey = rijndael.Key;
					}
					// See if the client provided an IV
					if(null == initVec)
					{ //Yes, have the alg create one
						initVec = rijndael.IV;
					}
					else
					{ //No, give it to the alg.
						rijndael.IV = initVec;
					}
					return rijndael.CreateEncryptor();
				} 

				default:
				{
					throw new CryptographicException("Algorithm ID '" + algorithmID + 
						"' not supported.");
				}
			}
		}
	}
}
