using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;
using tests.XUnitHelpers;
using Xunit;
using Xunit.Abstractions;

namespace tests
{
    public class ProductTest
    {
        private string _pactServiceUri = "http://127.0.0.1:9001";
        private string _baseUri = "https://ps.pactflow.io/pacts";
        private ITestOutputHelper _outputHelper { get; }

        public ProductTest(ITestOutputHelper output)
        {
            _outputHelper = output;
        }

        [Fact]
        public void EnsureProviderApiHonoursPactWithConsumer()
        {
            // Arrange
            var config = new PactVerifierConfig
            {
                // NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture the console output,
                // so a custom outputter is required.
                Outputters = new List<IOutput>
                {
                    new XUnitOutput(_outputHelper)
                }/*,
                //Verbose = true,
                PublishVerificationResults = true,
                ProviderVersion = System.Environment.GetEnvironmentVariable("GIT_COMMIT")*/
            };

            using (var _webHost = WebHost.CreateDefaultBuilder().UseStartup<TestStartup>().UseUrls(_pactServiceUri).Build())
            {
                _webHost.Start();

                //Act / Assert
                IPactVerifier pactVerifier = new PactVerifier(config);
                //var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "..", "pacts", "ApiClient-ProductService.json"));
                pactVerifier//.FromPactFile(pactFile)
                    .WithProviderStateUrl(new Uri($"{_pactServiceUri}/provider-states"))
                    .ServiceProvider("ProductService", new Uri(_pactServiceUri))
                    .HonoursPactWith("ApiClient")
                    // .FromPactBroker(new Uri($"{_baseUri}"),
                    //     uriOptions: new PactUriOptions(System.Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN")))
                    .FromPactUri(new Uri($"{_baseUri}/provider/ProductService/consumer/ApiClient/version/1.0.0"))
                    /*.FromPactBroker(new Uri($"{_baseUri}"), 
                        uriOptions: new PactUriOptions(System.Environment.GetEnvironmentVariable("PACT_BROKER_TOKEN")))*/
                        //consumerVersion: new List<string> { "1.0.0"})

                    .Verify();
            }
        }
    }
}