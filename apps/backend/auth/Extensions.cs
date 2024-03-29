namespace Flyingdarts.Backend.Auth;

using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;

    public static class APIGatewayCustomAuthorizerRequestExtensions
    {
        public static string GetValueOrDefault(this IDictionary<string, string> dictionary, string key, string defaultValue = null)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }

