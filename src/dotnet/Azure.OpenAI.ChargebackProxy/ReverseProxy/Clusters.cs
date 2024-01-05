using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Health;
using Yarp.ReverseProxy.LoadBalancing;

namespace Azure.OpenAI.ChargebackProxy.ReverseProxy
{
    public class Clusters
    {
        public static  IReadOnlyList<ClusterConfig> GetClusterConfig(IConfiguration config)
        {

            var primaryDestinations = config["AzureOpenAI:Endpoints"];
            var fallbackDestinations = config["AzureOpenAI:FallBackEndpoints"];



            List<ClusterConfig> clusters = new List<ClusterConfig> {
                Cluster(primaryDestinations, false),
                Cluster(fallbackDestinations, true)
            
            }; 
                



            return clusters.AsReadOnly();
            

        }

        public static ClusterConfig Cluster(string destinations, bool IsFallback)
        {
            var urls = JsonSerializer.Deserialize<List<string>>(destinations);
            
            string clusterName = "AzureOpenAI";
            if (IsFallback)
            {
                clusterName = "Fallback";
            }
           

            Dictionary<string, DestinationConfig> destinationDictionary = new Dictionary<string, DestinationConfig>();

            foreach (string destination in urls)
            {
                
                DestinationConfig destinationConfig = new()
                {
                    Address = destination
                    
                    
                };

                destinationDictionary[destination] = destinationConfig;
            }

            
            HealthCheckConfig healthCheckConfig = new()
            {

                Passive = new PassiveHealthCheckConfig() 
                {
                    Enabled = true,
                    Policy = HealthCheckConstants.PassivePolicy.TransportFailureRate,
                    ReactivationPeriod = TimeSpan.FromSeconds(30),
                    
                    
                }
            };



            ClusterConfig clusterConfig = new()
            {
                ClusterId = clusterName,
                Destinations = destinationDictionary,
                HealthCheck = healthCheckConfig,
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin
                
                
            };

            return clusterConfig;

        }
    }
}
