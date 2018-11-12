using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BraintreeCore.Api.Interfaces
{
    public interface IBraintreeConfig
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
