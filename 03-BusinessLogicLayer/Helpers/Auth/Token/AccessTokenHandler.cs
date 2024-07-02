using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;

namespace HolyShift
{
    public class AccessTokenCreator : IDisposable
    {
        protected ECDsaCng signer;

        public AccessTokenCreator(byte[] privateKey) { signer = new ECDsaCng(CngKey.Import(privateKey, CngKeyBlobFormat.EccPrivateBlob)); }

        public void Dispose() { signer?.Dispose(); signer = null; }

        public string Encode(params object[] values)
        {
            byte[] payload = JsonSerializer.SerializeToUtf8Bytes(values);
            return Convert.ToBase64String(signer.SignHash(payload).Concat(payload).ToArray(), Base64FormattingOptions.None).Replace('=', '~').Replace('+', '-').Replace('/', '_');
        }
    }

    public class AccessTokenVerifier : IDisposable
    {
        #region positionsClaims
        public static readonly long POSITION_DATE_EXPIRED = 0;
        public static readonly long POSITION_USERNAME = 1;
        public static readonly long POSITION_USERID = 2;
        public static readonly long POSITION_ROLE = 3;
        public static readonly long POSITION_ORGID = 4;
        #endregion

        protected ECDsaCng witness;

        public AccessTokenVerifier(byte[] publicKey) { witness = new ECDsaCng(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob)); }

        public void Dispose() { witness?.Dispose(); witness = null; }

        public bool IsValid(string token)
        {
            try
            {
                byte[] binary = Convert.FromBase64String(token.Replace('~', '=').Replace('-', '+').Replace('_', '/'));
                return witness.VerifyHash(binary.Skip(64).ToArray(), binary.Take(64).ToArray());
            }
            catch { return false; }
        }

        public static JsonElement[] Decode(string token)
        { return JsonSerializer.Deserialize<JsonElement[]>(Convert.FromBase64String(token.Replace('~', '=').Replace('-', '+').Replace('_', '/')).Skip(64).ToArray()); }

        #region parseInnerJsonElement
        public static DateTime GetExpirationDate(JsonElement[] array)
        {
            return array[POSITION_DATE_EXPIRED].GetDateTime();
        }
        public static string GetUserEmail(JsonElement[] array)
        {
            return array[POSITION_USERNAME].GetString();
        }
        public static string GetUserId(JsonElement[] array)
        {
            return array[POSITION_USERID].GetString();
        }
        public static string GetUserRole(JsonElement[] array)
        {
            return array[POSITION_ROLE].GetString();
        }
        public static string GetOrganizationId(JsonElement[] array)
        {
            return array[POSITION_ORGID].GetString();
        }
        #endregion
    }

    public class AccessTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static readonly AccessTokenVerifier s_verifier;

        static AccessTokenHandler()
        {
            using (System.IO.Stream pk = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("HolyShift.PublicKey"))
            { byte[] blob = new byte[pk.Length]; pk.Read(blob, 0, blob.Length); s_verifier = new AccessTokenVerifier(blob); }
        }

        [Obsolete]
        public AccessTokenHandler(Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            System.Text.Encodings.Web.UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string x = Request.Headers["Authorization"];
            string token = System.Net.Http.Headers.AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out System.Net.Http.Headers.AuthenticationHeaderValue auth) &&
              string.Equals(auth.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase) ? auth.Parameter : Request.Query["Bearer"];
            if (string.IsNullOrWhiteSpace(token) || !s_verifier.IsValid(token)) return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));


            System.Text.Json.JsonElement[] info = AccessTokenVerifier.Decode(token);
            if (info.Length > 0 && DateTime.UtcNow > info[0].GetDateTime()) return Task.FromResult(AuthenticateResult.Fail("Expired"));

            System.Collections.Generic.List<Claim> claims = new System.Collections.Generic.List<Claim>();
            if (info.Length > 0) claims.Add(new Claim(ClaimTypes.Expiration, info[0].GetString()));
            if (info.Length > 1) claims.Add(new Claim(ClaimTypes.Email, info[1].GetString()));
            if (info.Length > 2) claims.Add(new Claim("UserId", info[2].GetString()));
            if (info.Length > 3) claims.Add(new Claim(ClaimTypes.Role, info[3].GetString()));
            if (info.Length > 4) claims.Add(new Claim("OrgId", info[4].GetString()));


            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
        }
    }
}

