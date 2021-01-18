using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Http.Console
{
    public class JsonHttpClient
    {
        private readonly HttpClient _httpClient;

        public JsonHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string url)
        {
            return GetAsync<TResult>(url, string.Empty);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="query">查询字符串</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string url, IDictionary<string, string> query)
        {
            return GetAsync<TResult>(url, query, string.Empty);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="jwtToken">Token</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(string url, string jwtToken)
        {
            return GetAsync<TResult>(url, null, jwtToken);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="query">查询字符串</param>
        /// <param name="jwtToken">Token</param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(string url, IDictionary<string, string> query, string jwtToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            url = FormatQuery(url, query);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrEmpty(jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(resultJson);
            }

            return default;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="query">查询字符串</param>
        /// <param name="headers">请求头</param>
        /// <returns></returns>
        public async Task<TResult> GetAsync<TResult>(string url, IDictionary<string, string> query, IDictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            url = FormatQuery(url, query);

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(resultJson);
            }

            return default;
        }

        private string FormatQuery(string url, IDictionary<string, string> query)
        {
            if (query != null)
            {
                var builder = new StringBuilder(url);
                builder.Append("?");

                foreach (var q in query)
                {
                    builder.Append($"{q.Key}={q.Value}&");
                }

                url = builder.ToString().TrimEnd('&');
            }

            return url;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="data">Body数据</param>
        /// <returns></returns>
        public Task<TResult> PostAsync<TResult>(string url, object data)
        {
            return PostAsync<TResult>(url, data, string.Empty);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="TResult">结果对象</typeparam>
        /// <param name="url">请求路径</param>
        /// <param name="data">Body数据</param>
        /// <param name="jwtToken">Token</param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string url, object data, string jwtToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            var request = new HttpRequestMessage(HttpMethod.Post, url);

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);

                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
            }

            if (!string.IsNullOrEmpty(jwtToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(content);
            }

            return default;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <typeparam name="TResult">请求结果</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        public async Task<TResult> PostAsync<TResult>(string url, object data, IDictionary<string, string> headers)
            where TResult : class
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var json = JsonConvert.SerializeObject(data);
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url))
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResult>(content);
            }

            return default;
        }
    }
}
