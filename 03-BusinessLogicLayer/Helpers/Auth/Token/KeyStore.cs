using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace HolyShift
{
    public static class KeyStore
    {

        public const string PUBLIC_KEY = "PublicKey";

        public const string PRIVATE_KEY = "PrivateKey";


        private static byte[] _publicKey;
        public static byte[] PublicKey
        {
            get
            {
                if (_publicKey == null)
                {
                    using (System.IO.Stream pk = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HolyShift.PublicKey"))
                    { byte[] blob = new byte[pk.Length]; pk.Read(blob, 0, blob.Length); _publicKey = blob; }
                }
                return _publicKey;
            }
        }
        private static byte[] _privateKey;

        public static byte[] PrivateKey
        {
            get
            {
                if (_privateKey == null)
                {
                    using (System.IO.Stream pk = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HolyShift.PrivateKey"))
                    { byte[] blob = new byte[pk.Length]; pk.Read(blob, 0, blob.Length); _privateKey = blob; }
                }
                return _privateKey;
            }
        }
    }
}
