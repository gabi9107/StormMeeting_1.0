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
 
* Neither the name of Marc Clifton nor the names of its contributors may be
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

using System;
using System.Security.Cryptography;

namespace Clifton.Tools.Data
{
	public class DecryptTransformer
	{
		private EncryptionAlgorithm algorithmID;
		private byte[] initVec;

		public byte[] IV
		{
			set {initVec = value;}
		}

		public DecryptTransformer(EncryptionAlgorithm deCryptId)
		{
			algorithmID = deCryptId;
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
					des.Key = bytesKey;
					des.IV = initVec;
					return des.CreateDecryptor();
				}
				case EncryptionAlgorithm.TripleDes:
				{
					TripleDES des3 = new TripleDESCryptoServiceProvider();
					des3.Mode = CipherMode.CBC;
					return des3.CreateDecryptor(bytesKey, initVec);
				}
				case EncryptionAlgorithm.Rc2:
				{
					RC2 rc2 = new RC2CryptoServiceProvider();
					rc2.Mode = CipherMode.CBC;
					return rc2.CreateDecryptor(bytesKey, initVec);
				}
				case EncryptionAlgorithm.Rijndael:
				{
					Rijndael rijndael = new RijndaelManaged();
					rijndael.Mode = CipherMode.CBC;
					return rijndael.CreateDecryptor(bytesKey, initVec);
				} 
				default:
				{
					throw new CryptographicException("Algorithm ID '" + algorithmID + 
						"' not supported.");
				}
			}
		} //end GetCryptoServiceProvider
  

	}
}