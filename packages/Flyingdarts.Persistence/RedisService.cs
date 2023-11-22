

using System.Text.Json;
using Amazon.ElastiCache;
using Amazon.ElastiCache.Model;
using StackExchange.Redis;

public interface IRedisService
{
    IDatabase GetDatabase();
}

public class RedisService : IRedisService
{
    private ConnectionMultiplexer GetConnection(string url)
    {
        var config = new ConfigurationOptions
        {
            AllowAdmin = true,
            AbortOnConnectFail = false,
        };

        config.EndPoints.Add(url, 6379);

        return ConnectionMultiplexer.Connect(config);
    }
    public IDatabase GetDatabase()
    {
        var client = new AmazonElastiCacheClient();
        // Replace "your-cache-cluster-id" with your actual ElastiCache cluster ID
        string cacheClusterId = "flyingdarts-redis-0001-001";

        // Example: Describe ElastiCache cluster
        DescribeCacheClustersRequest describeRequest = new DescribeCacheClustersRequest
        {
            CacheClusterId = cacheClusterId,
            ShowCacheNodeInfo = true
        };

        DescribeCacheClustersResponse describeResponse = client.DescribeCacheClustersAsync(describeRequest).Result;

        if (describeResponse.CacheClusters.Count > 0)
        {
            CacheCluster cacheCluster = describeResponse.CacheClusters[0];
            Console.WriteLine($"{JsonSerializer.Serialize(cacheCluster)}");
            Console.WriteLine($"ElastiCache Cluster ID: {cacheCluster.CacheClusterId}");
            Console.WriteLine($"Status: {cacheCluster.CacheClusterStatus}");
            Console.WriteLine($"Engine: {cacheCluster.Engine}");
            return GetConnection(cacheCluster.CacheNodes.First().Endpoint.Address).GetDatabase();
        }
        else
        {
            throw new Exception($"ElastiCache Cluster with ID {cacheClusterId} not found.");
        }
    }
}