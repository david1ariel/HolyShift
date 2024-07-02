using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace HolyShift
{
    public class EncryptDecryptHelper
    {
        public static async Task<byte[]> Encrypt(System.IO.Stream source, System.IO.Stream target, CancellationToken ct = default)
        {
            using (AesCng encryptor = new AesCng { IV = new byte[16] })
            {
                CryptoStream cipher = new CryptoStream
                    (target, encryptor.CreateEncryptor(), CryptoStreamMode.Write, true);
                await using (cipher.ConfigureAwait(false)) await source.CopyToAsync(cipher, ct).ConfigureAwait(false);
                return encryptor.Key;
            }
        }

        /// <summary>decrypt stream with an AES key</summary>
        public static async Task Decrypt(System.IO.Stream source, System.IO.Stream target, byte[] key, CancellationToken ct = default)
        {
            using (AesCng decryptor = new AesCng { Key = key, IV = new byte[16] })
            {
                CryptoStream cipher = new CryptoStream
                    (source, decryptor.CreateDecryptor(), CryptoStreamMode.Read, true);
                await using (cipher.ConfigureAwait(false)) await cipher.CopyToAsync(target, ct).ConfigureAwait(false);
            }
        }

        private static DerObjectIdentifier id = SecObjectIdentifiers.SecP192r1;

        /// <summary>how to encrypt blob - elliptic curve OID</summary>
        public static string Parameters { get { return id.Id; } set { id = new DerObjectIdentifier(value); } }

        /// <summary>derive elliptic-curve public key from a password</summary>
        public static byte[] DerivePublicKey(string password)
        {
            // seed random generator with password
            SecureRandom hasher = SecureRandom.GetInstance("SHA256PRNG", false);
            hasher.SetSeed(new System.Text.UTF8Encoding().GetBytes(password));

            // derive EC key pair from the password
            ECKeyPairGenerator factory = new ECKeyPairGenerator("EC");
            factory.Init(new ECKeyGenerationParameters(id, hasher));
            return ((ECPublicKeyParameters)factory.GenerateKeyPair().Public).Q.GetEncoded(true);
        }

        /// <summary>encrypt blob with elliptic-curve public key and random salt</summary>
        public static byte[] Encrypt(byte[] plaintext, byte[] pubKey)
        {
            SM2Engine sm2Engine = new SM2Engine();
            sm2Engine.Init(true, new ParametersWithRandom
                (new ECPublicKeyParameters("EC", CustomNamedCurves.GetByOid(id).Curve.DecodePoint(pubKey), id)));
            return sm2Engine.ProcessBlock(plaintext, 0, plaintext.Length);
        }

        /// <summary>decrypt blob with elliptic-curve private key, derived from a password</summary>
        public static byte[] Decrypt(byte[] ciphertext, string password)
        {
            // seed random generator with password
            SecureRandom hasher = SecureRandom.GetInstance("SHA256PRNG", false);
            hasher.SetSeed(new System.Text.UTF8Encoding().GetBytes(password));

            // derive EC key pair from the password
            ECKeyPairGenerator factory = new ECKeyPairGenerator("EC");
            factory.Init(new ECKeyGenerationParameters(id, hasher));

            // decrypt with private key
            SM2Engine sm2Engine = new SM2Engine();
            sm2Engine.Init(false, factory.GenerateKeyPair().Private);
            return sm2Engine.ProcessBlock(ciphertext, 0, ciphertext.Length);
        }
    }
}
