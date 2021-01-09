using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tenlastic {
    public class LoginService {

        #pragma warning disable 0649
        [Serializable]
        private struct Key {
            public string alg;
            public string kwt;
            public string use;
            public string[] x5c;
        }

        [Serializable]
        private struct LogInResponse {
            public string accessToken;
            public string refreshToken;
        }

        [Serializable]
        private struct PublicKeyResponse {
            public Key[] keys;
        }
        #pragma warning restore 0649

        public static readonly LoginService singleton = new LoginService();

        private readonly HttpManager httpManager = HttpManager.singleton;

        public async Task CreateWithCredentials(string email, string password) {
            JObject parameters = new JObject {
                { "email", email },
                { "password", password }
            };

            LogInResponse response = await httpManager.Request<LogInResponse>(
                HttpMethod.Post,
                EnvironmentManager.singleton.environmentObject.loginApiBaseUrl,
                parameters
            );

            TokenManager.singleton.accessToken = response.accessToken;
            TokenManager.singleton.refreshToken = response.refreshToken;

            Jwt jwt = new Jwt(response.accessToken);
            GameState.singleton.userModel = jwt.payload.user;
        }

        public async Task CreateWithRefreshToken(string token) {
            JObject parameters = new JObject {
                { "token", token }
            };

            LogInResponse response = await httpManager.Request<LogInResponse>(
                HttpMethod.Post,
                EnvironmentManager.singleton.environmentObject.loginApiBaseUrl + "/refresh-token",
                parameters,
                true
            );

            TokenManager.singleton.accessToken = response.accessToken;
            TokenManager.singleton.refreshToken = response.refreshToken;

            Jwt jwt = new Jwt(response.accessToken);
            GameState.singleton.userModel = jwt.payload.user;
        }

        public async Task LogOut() {
            await httpManager.Request(
                HttpMethod.Delete,
                EnvironmentManager.singleton.environmentObject.loginApiBaseUrl
            );

            GameState.singleton.userModel = null;
            TokenManager.singleton.Clear();
        }

        public async Task<bool> Validate(string accessToken) {
            PublicKeyResponse response = await httpManager.Request<PublicKeyResponse>(
                HttpMethod.Get,
                EnvironmentManager.singleton.environmentObject.publicKeyApiBaseUrl + "/jwt"
            );

            byte[] keyBytes = Convert.FromBase64String(response.keys[0].x5c[0]);

            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            RSAParameters rsaParameters = new RSAParameters();
            rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            string[] parts = accessToken.Split('.');
            SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(parts[0] + '.' + parts[1]));

            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("SHA256");

            return rsaDeformatter.VerifySignature(hash, Base64UrlDecode(parts[2]));
        }

        private static byte[] Base64UrlDecode(string input) {
            string output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding

            switch (output.Length % 4) {
                // Pad with trailing '='s
                case 0: 
                    break;

                case 1: 
                    output += "==="; 
                    break; 

                case 2: 
                    output += "=="; 
                    break;

                case 3: 
                    output += "="; 
                    break;

                default: 
                    throw new Exception("Invalid base64 string!");
            }

            return Convert.FromBase64String(output);
        }

    }
}
