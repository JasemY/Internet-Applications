using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Ia.Cl.Model.Cryptography
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Symmetric Key Algorithm (Rijndael/AES) to encrypt and decrypt data.
    /// </summary>
    /// <value>
    /// Symmetric Key Algorithm (Rijndael/AES) to encrypt and decrypt data. As long as encryption and decryption routines use the same
    /// parameters to generate the keys, the keys are guaranteed to be the same. The class uses static functions with duplicate code to make it easier to
    /// demonstrate encryption and decryption logic. In a real-life application, this may not be the most efficient way of handling encryption, so - as
    /// soon as you feel comfortable with it - you may want to redesign this class.
    /// 
    /// To create a new certificate (.NET Framework Tools Certificate Creation Tool (Makecert.exe) http://msdn.microsoft.com/en-us/library/bfsktky3(VS.71).aspx)
    /// Do the following:
    /// - "C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\makecert" -r -pe -n "CN=Test Client Certificate" -b 01/01/2000 -e 01/01/2036 -eku 1.3.6.1.5.5.7.3.1 -ss my -sr CurrentUser -sky exchange -sp "Microsoft RSA SChannel Cryptographic Provider" -sy 12
    /// - Unless, that is, TLS-PSK, the Secure Remote Password (SRP) protocol, or some other protocol is used that can provide strong mutual authentication in the absence of certificates.
    /// - In practice, you will hash the message beforehand (with hash algorithm such as MD5 or SHA1), obtaining the hashed message M1. Then you will encrypt M1 with your private key K0, digitally signing your message, and, finally, you will send your message M, the encrypted hash M1 (the signature) and the public key K1 to your recipient. Your recipient will compute the hash of your message M and will compare it with the decrypted value of M1. If the two hashes matches, the signature is valid.
    /// - Launch the command "certmgr.msc" in Windows Start/Start Search to open the Certificate Manager.
    /// - Double click a certifical file *.pfx to import it into the certificate store and you can use its public keys
    /// For information see:
    /// http://www.codeproject.com/KB/cs/Data_Encryption_Decryptio.aspx
    /// http://www.grimes.demon.co.uk/workshops/secWSSixteen.htm
    /// 
    /// The other party should have a "public key certificate" that I import into my certificate store
    /// 
    /// Import a Certificate:
    /// - You should only import certificates obtained from trusted sources. Importing an unreliable certificate could compromise the security of any system component that uses the imported certificate.
    /// - You can import a certificate into any logical or physical store. In most cases, you will import certificates into the Personal store or the Trusted Root Certification Authorities store, depending on whether the certificate is intended for you or if it is a root CA certificate.
    /// 
    /// http://www.entrust.com/resources/pdf/cryptointro.pdf
    /// - Add certificate name in web.config
    /// </value>
    /// <remarks> 
    /// Copyright © 2001-2015 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
    ///
    /// This library is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
    /// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
    ///
    /// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
    /// 
    /// You should have received a copy of the GNU General Public License along with this library. If not, see http://www.gnu.org/licenses.
    /// 
    /// Copyright notice: This notice may not be removed or altered from any source distribution.
    /// </remarks> 
    public class Encryption
    {
        static string certificateName = ConfigurationManager.AppSettings["certificateName"].ToString();

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Encryption methods common to most applications - focus is on standard configuration using RSA symetric keys of 1024 bytes or higher.
        /// </summary>
        /// <remark link="http://www.grimes.demon.co.uk/workshops/secWSSixteen.htm"/>
        /// <remark link="http://www.entrust.com/resources/pdf/cryptointro.pdf"/>
        public Encryption() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// </summary>
        /// <remarks> </remarks>
        public static string Sign(string data)
        {
            bool verify;
            byte[] buffer, signature;
            string s;
            X509Certificate2 certificate;

            s = string.Empty;
            certificate = null;

            certificate = GetCertificate(certificateName);

            if (certificate == null) { throw new Exception("No Certificate could be found in name " + certificateName); }
            else
            {
                if (certificate.HasPrivateKey)
                {
                    // cast the privateKey object to a RSACryptoServiceProvider object, or, in more elegant way:
                    RSACryptoServiceProvider privateKey = certificate.PrivateKey as RSACryptoServiceProvider;

                    // now a signature can be performed. To do so, the SignData method of the privateKey object can be used. It accepts, as input, (1) the data to sign, as array of bytes, and (2) the object that represents the hash algorithm to use:
                    buffer = Encoding.Default.GetBytes(data);

                    signature = privateKey.SignData(buffer, new SHA1Managed());

                    // signature can also be verified. To do so you must utilize the public key of the certificate.
                    RSACryptoServiceProvider publicKey = certificate.PublicKey.Key as RSACryptoServiceProvider;
                    verify = publicKey.VerifyData(buffer, new SHA1Managed(), signature);

                    if (verify) s = Convert.ToBase64String(signature);
                    else s = null;
                }
                else
                {
                    throw new Exception("Certificate used for has no private key.");
                }
            }

            s = "<dataSignature><data>" + data + "</data><signature>" + s + "</signature></dataSignature>";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <param name="recipientCertificate"></param>
        /// <returns></returns>
        public static string Seal(string data, string recipientCertificate)
        {
            string s, cipher, key, iv;
            X509Certificate2 certificate;

            s = key = iv = string.Empty;
            certificate = null;

            System.Security.Cryptography.Rijndael r = System.Security.Cryptography.Rijndael.Create();

            cipher = global::Ia.Cl.Model.Cryptography.Rijndael.Encrypt(data, r.Key, r.IV);

            certificate = GetCertificate(recipientCertificate);

            if (certificate == null) { throw new Exception("No Certificate could be found in name " + recipientCertificate); }
            else
            {
                byte[] cipher_byte;

                using (var rsa = (RSACryptoServiceProvider)certificate.PublicKey.Key)
                {
                    cipher_byte = rsa.Encrypt(r.Key, false);
                    key = Convert.ToBase64String(cipher_byte);

                    cipher_byte = rsa.Encrypt(r.IV, false);
                    iv = Convert.ToBase64String(cipher_byte);
                }
            }

            return s = "<cipherSymmetricKey><cipher>" + cipher + "</cipher><symmetricKey><key>" + key + "</key><iv>" + iv + "</iv></symmetricKey></cipherSymmetricKey>";
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void Deliver(string data)
        {
            // send data 
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static string Accept()
        {
            // accept data 
            string s;

            s = null;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Open(string data)
        {
            byte[] key_, iv_;
            string s, cipher, key, iv;
            XmlDocument xd;
            X509Certificate2 certificate;

            key_ = iv_ = null;
            s = string.Empty;
            certificate = null;

            certificate = GetCertificate(certificateName);

            xd = new XmlDocument();

            xd.LoadXml(data);

            cipher = xd.SelectSingleNode("cipherSymmetricKey/cipher").InnerText;
            key = xd.SelectSingleNode("cipherSymmetricKey/symmetricKey/key").InnerText;
            iv = xd.SelectSingleNode("cipherSymmetricKey/symmetricKey/iv").InnerText;

            if (certificate == null) { throw new Exception("No Certificate could be found in name " + certificateName); }
            else
            {
                key_ = Convert.FromBase64String(key);
                iv_ = Convert.FromBase64String(iv);

                using (var rsa = (RSACryptoServiceProvider)certificate.PrivateKey)
                {
                    key_ = rsa.Decrypt(key_, false);
                    iv_ = rsa.Decrypt(iv_, false);
                }
            }

            System.Security.Cryptography.Rijndael r = System.Security.Cryptography.Rijndael.Create();

            s = global::Ia.Cl.Model.Cryptography.Rijndael.Decrypt(cipher, key_, iv_);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Verify(string data)
        {
            bool verify;
            byte[] buffer, si;
            string s, u, signature;
            XmlDocument xd;
            X509Certificate2 certificate;

            s = string.Empty;
            certificate = null;

            certificate = GetCertificate(certificateName);

            xd = new XmlDocument();

            xd.LoadXml(data);

            u = xd.SelectSingleNode("dataSignature/data").InnerText;
            signature = xd.SelectSingleNode("dataSignature/signature").InnerText;

            if (certificate == null) { throw new Exception("No Certificate could be found in name " + certificateName); }
            else
            {
                // cast the publicKey object to a RSACryptoServiceProvider object, or, in more elegant way:
                RSACryptoServiceProvider publiceKey = certificate.PublicKey.Key as RSACryptoServiceProvider;

                // now a signature can be performed. To do so, the SignData method of the privateKey object can be used. It accepts, as input, (1) the data to sign, as array of bytes, and (2) the object that represents the hash algorithm to use:
                buffer = Encoding.Default.GetBytes(u);

                //si = Encoding.Default.GetBytes(signature);
                si = Convert.FromBase64String(signature);

                verify = publiceKey.VerifyData(buffer, new SHA1Managed(), si);
            }

            s = "<data>" + u + "</data>";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Encrypted the specified data.
        /// </summary>
        /// <param name="data">The bytes to encrypt</param>
        /// <returns>Returns the encrypted bytes.</returns>
        public static string Encrypt(string data)
        {
            string s;
            X509Certificate2 certificate;

            s = string.Empty;
            certificate = null;

            certificate = GetCertificate(certificateName);

            if (certificate == null) { throw new Exception("No Certificate could be found in name " + certificateName); }
            else
            {
                try
                {
                    string PlainString = data.Trim();
                    byte[] cipherbytes = ASCIIEncoding.ASCII.GetBytes(PlainString);
                    byte[] cipher;

                    using (var rsa = (RSACryptoServiceProvider)certificate.PublicKey.Key) cipher = rsa.Encrypt(cipherbytes, false);

                    s = Convert.ToBase64String(cipher);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Decrypts the bytes specified.
        /// </summary>
        /// <param name="data">The bytes to decrypt</param>
        /// <returns>Returns the decrypted bytes.</returns>
        public static string Decrypt(string data)
        {
            string s;
            X509Certificate2 certificate;

            s = string.Empty;
            certificate = null;

            certificate = GetCertificate(certificateName);

            if (certificate == null) { throw new Exception("No Certificate could be found in name " + certificateName); }
            else
            {
                try
                {
                    byte[] cipherbytes = Convert.FromBase64String(data);

                    if (certificate.HasPrivateKey)
                    {
                        byte[] plainbytes;

                        using (RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey) plainbytes = rsa.Decrypt(cipherbytes, false);

                        System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

                        s = enc.GetString(plainbytes);
                    }
                    else
                    {
                        throw new Exception("Certificate used for has no private key.");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the certificate in X509Certificate2 format from its string name
        /// </summary>
        /// <param name="name">Certificate name</param>
        /// <returns>Certificate</returns>
        private static X509Certificate2 GetCertificate(string name)
        {
            X509Store store;
            X509Certificate2 certificate;

            certificate = null;

            store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadWrite);

            if (name.Length > 0)
            {
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    if (cert.SubjectName.Name.Contains(name))
                    {
                        certificate = cert;
                        break;
                    }
                }
            }
            else
            {
                certificate = store.Certificates[0];
            }

            return certificate;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// This class uses a symmetric key algorithm (Rijndael/AES) to encrypt and decrypt data. As long as encryption and decryption routines use the same
    /// parameters to generate the keys, the keys are guaranteed to be the same. The class uses static functions with duplicate code to make it easier to
    /// demonstrate encryption and decryption logic. In a real-life application, this may not be the most efficient way of handling encryption, so - as
    /// soon as you feel comfortable with it - you may want to redesign this class.
    /// </summary>
    public class Rijndael
    {
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Encrypts specified plaintext using Rijndael symmetric key algorithm
        /// and returns a base64-encoded result.
        /// </summary>
        /// <param name="plainText">
        /// Plaintext value to be encrypted.
        /// </param>
        /// <param name="initVectorBytes">
        /// Initialization vector (or IV). This value is required to encrypt the first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keyBytes"></param>
        /// <returns>
        /// Encrypted value formatted as a base64-encoded string.
        /// </returns>
        public static string Encrypt(string plainText, byte[] keyBytes, byte[] initVectorBytes)
        {
            // convert our plaintext into a byte array. Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization vector. Key size will be defined based on the number of the key bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipher = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipher;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        /// </summary>
        /// <param name="cipher">
        /// Base64-formatted ciphertext value.
        /// </param>
        /// <param name="initVectorBytes">
        /// Initialization vector (or IV). This value is required to encrypt the first block of plaintext data. For RijndaelManaged class IV must be exactly 16 ASCII characters long.
        /// </param>
        /// <param name="keyBytes"></param>
        /// <returns>
        /// Decrypted string value.
        /// </returns>
        /// </summary>
        /// <remarks> 
        /// Most of the logic in this function is similar to the Encrypt logic. In order for decryption to work, all parameters of this function
        /// - except cipher value - must match the corresponding parameters of the Encrypt function which was called to generate the ciphertext.
        /// </remarks>
        public static string Decrypt(string cipher, byte[] keyBytes, byte[] initVectorBytes)
        {
            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipher);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization vector. Key size will be defined based on the number of the key bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data will be, allocate the buffer long enough to hold ciphertext; plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static byte[] Password()
        {
            /// <param name="passPhrase">
            /// Passphrase from which a pseudo-random password will be derived. The
            /// derived password will be used to generate the encryption key.
            /// Passphrase can be any string. In this example we assume that this
            /// passphrase is an ASCII string.
            /// </param>
            /// <param name="saltValue">
            /// Salt value used along with passphrase to generate password. Salt can
            /// be any string. In this example we assume that salt is an ASCII string.
            /// </param>
            /// <param name="hashAlgorithm">
            /// Hash algorithm used to generate password. Allowed values are: "MD5" and
            /// "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
            /// </param>
            /// <param name="passwordIterations">
            /// Number of iterations used to generate password. One or two iterations
            /// should be enough.
            /// </param>
            /// <param name="keySize">
            /// Size of encryption key in bits. Allowed values are: 128, 192, and 256.
            /// Longer keys are more secure than shorter keys.
            /// </param>

            string passPhrase = "*";        // can be any string
            string saltValue = "*";        // can be any string
            string hashAlgorithm = "SHA1";             // can be "MD5"
            int passwordIterations = 2;                  // can be any number
            int keySize = 256;                // can be 192 or 128

            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            return keyBytes;
        }
        */

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    public class Md5
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static string Hash(string text)
        {
            // hash an input string and return the hash as a 32 character hexadecimal string

            byte[] data;
            StringBuilder sb;
            MD5 md5;

            sb = new StringBuilder();
            md5 = MD5.Create();

            // convert the input string to a byte array and compute the hash
            data = md5.ComputeHash(Encoding.Default.GetBytes(text));

            // loop through each byte of the hashed data and format each one as a hexadecimal string
            for (int i = 0; i < data.Length; i++) sb.Append(data[i].ToString("x2"));

            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}

