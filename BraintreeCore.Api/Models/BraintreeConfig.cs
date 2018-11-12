using Braintree;
using BraintreeCore.Api.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreeCore.Api.Models
{
    public class BraintreeConfig : IBraintreeConfig
    {
        private IConfiguration _config;

        public BraintreeConfig(IConfiguration config)
        {
            _config = config;
        }

        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }

        public IBraintreeGateway CreateGateway()
        {
            if (MerchantId == null || PublicKey == null || PrivateKey == null)
            {
                Environment = _config["Braintree:Environment"];
                MerchantId = _config["Braintree:MerchantId"];
                PublicKey = _config["Braintree:PublicKey"];
                PrivateKey = _config["Braintree:PrivateKey"];
            }

            return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
        }
    }
}
