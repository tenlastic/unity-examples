using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

namespace Tenlastic {
    public class HttpManager {

        public static readonly HttpManager singleton = new HttpManager();

        public delegate void OnUnauthorizedRequestDelegate();
        public event OnUnauthorizedRequestDelegate OnUnauthorizedRequest;

        private readonly HttpClient httpClient = new HttpClient();
        private bool isRefreshingToken;
        private readonly LoginService loginService = LoginService.singleton;

        public async Task<string> Request(HttpMethod method, string url, JObject parameters = null, bool skipRefreshToken = false) {
            if (!skipRefreshToken) {
                await RefreshAccessToken();
            }

            StringContent content = new StringContent(parameters?.ToString(Formatting.None));

            NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
            query.Add("query", parameters.ToString(Formatting.None));

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Add("Authorization", string.Format("Bearer {0}", TokenManager.singleton.accessToken));
            httpRequestMessage.Headers.Add("Content-Type", "application/json");
            httpRequestMessage.Method = method;

            if (method == HttpMethod.Post || method == HttpMethod.Put) {
                httpRequestMessage.Content = content;
                httpRequestMessage.RequestUri = new Uri(url);
            } else {
                httpRequestMessage.RequestUri = new Uri(url + query.ToString());
            }

            HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessage);
            string body = await response.Content.ReadAsStringAsync();
            int statusCode = (int)response.StatusCode;

            switch (statusCode) {
                case 200:
                    return body;

                case 401:
                    TokenManager.singleton.Clear();
                    throw new HttpException(401, "Unauthorized.");

                case 403:
                    throw new HttpException(403, "Forbidden.");

                default:
                    HttpException.HttpErrors errors = JsonUtility.FromJson<HttpException.HttpErrors>(body);
                    throw new HttpException(statusCode, errors.errors);
            }
        }

        public async Task<T> Request<T>(HttpMethod method, string url, JObject parameters = null, bool skipRefreshToken = false) {
            string body = await Request(method, url, parameters, skipRefreshToken);
            return JsonConvert.DeserializeObject<T>(body);
        }

        private async Task RefreshAccessToken() {
            await WaitUntil(() => !isRefreshingToken, 25, 5000);

            if (string.IsNullOrEmpty(TokenManager.singleton.accessToken) || string.IsNullOrEmpty(TokenManager.singleton.refreshToken)) {
                return;
            }

            Jwt accessToken = new Jwt(TokenManager.singleton.accessToken);
            if (!accessToken.isExpired) {
                return;
            }

            Jwt refreshToken = new Jwt(TokenManager.singleton.refreshToken);
            if (refreshToken.isExpired) {
                OnUnauthorizedRequest.Invoke();
                return;
            }

            isRefreshingToken = true;

            try {
                await loginService.CreateWithRefreshToken(TokenManager.singleton.refreshToken);
            } catch (HttpException) {
                OnUnauthorizedRequest.Invoke();
            }

            isRefreshingToken = false;
        }

        private async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1) {
            Task waitTask = Task.Run(async () => {
                while (!condition()) {
                    await Task.Delay(frequency);
                }
            });

            Task result = await Task.WhenAny(waitTask, Task.Delay(timeout));
            if (waitTask != result) {
                throw new TimeoutException();
            }
        }

    }
}
