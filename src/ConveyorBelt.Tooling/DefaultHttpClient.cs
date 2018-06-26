﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConveyorBelt.Tooling
{
    public class DefaultHttpClient : IHttpClient
    {

        private HttpClient _client = null;

        public DefaultHttpClient(IEnumerable<KeyValuePair<string, string>> defaultHeaders = null)
        {
            var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, errors) => true; // accept any cert
            _client = new HttpClient(handler);
            if (defaultHeaders != null)
            {
                foreach (var header in defaultHeaders)
                    _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return _client.PostAsync(requestUri, content);
        }

        public Task<HttpResponseMessage> GetAsync(string uri)
        {
            return _client.GetAsync(uri);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync(string requestUri, string payload)
        {
            /*JsonConvert.ToString if necessary ?*/
            return _client.PutAsync(requestUri, new StringContent(payload, Encoding.UTF8, "application/json"));
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            return _client.PutAsync(requestUri, content);
        }
    }
}
