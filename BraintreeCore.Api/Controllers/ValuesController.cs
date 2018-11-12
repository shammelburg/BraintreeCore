using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using BraintreeCore.Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BraintreeCore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ValuesController : Controller
    {
        public static readonly TransactionStatus[] transactionSuccessStatuses = {
                                                                                    TransactionStatus.AUTHORIZED,
                                                                                    TransactionStatus.AUTHORIZING,
                                                                                    TransactionStatus.SETTLED,
                                                                                    TransactionStatus.SETTLING,
                                                                                    TransactionStatus.SETTLEMENT_CONFIRMED,
                                                                                    TransactionStatus.SETTLEMENT_PENDING,
                                                                                    TransactionStatus.SUBMITTED_FOR_SETTLEMENT

                                                               };

        IBraintreeConfig _bconfig;

        public ValuesController(IBraintreeConfig bconfig)
        {
            _bconfig = bconfig;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Token()
        {
            var gateway = _bconfig.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            return Ok(clientToken);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Create(IFormCollection Request)
        {
            var gateway = _bconfig.GetGateway();
            Decimal amount;

            try
            {
                amount = Convert.ToDecimal(Request["amount"]);
            }
            catch (FormatException e)
            {
                return BadRequest("Error: 81503: Amount is an invalid format.");
            }

            var nonce = Request["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                Transaction transaction = result.Target;
                return Ok(new { id = transaction.Id });
            }
            else if (result.Transaction != null)
            {
                return Ok(new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }

                return BadRequest(errorMessages);
            }
        }

        public IActionResult Show(String id)
        {
            var gateway = _bconfig.GetGateway();
            Transaction transaction = gateway.Transaction.Find(id);
            ShowResponse show = new ShowResponse();

            if (transactionSuccessStatuses.Contains(transaction.Status))
            {
                show.Header = "Sweet Success!";
                show.Icon = "success";
                show.Message = "Your test transaction has been successfully processed. See the Braintree API response and try again.";
            }
            else
            {
                show.Header = "Transaction Failed";
                show.Icon = "fail";
                show.Message = "Your test transaction has a status of " + transaction.Status + ". See the Braintree API response and try again.";
            };

            return Ok(new
            {
                transaction = transaction,
                show = show
            });
        }
    }

    public class ShowResponse
    {
        public string Header { get; set; }
        public string Icon { get; set; }
        public string Message { get; set; }
    }
}
