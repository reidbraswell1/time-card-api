using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TimeCard.Shared;
namespace TimeCard.Controllers
{
    //[Route("api/pay")]
    public class PayApiController : Controller
    {

        // For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
        private readonly IPayRepository _payRepo;
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _configuration;
        private readonly String _getAuthKey;
        private readonly String _postAuthKey;
        private readonly String _putAuthKey;
        private readonly String _deleteAuthKey;

        public PayApiController(IPayRepository payRepo, IAuthRepository authRepo, IConfiguration configuration)
        {
            _payRepo = payRepo;
            _authRepo = authRepo;
            _configuration = configuration;
            _getAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Post").Value;
            _postAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Post").Value;
            _putAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Put").Value;
            _deleteAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Delete").Value;
        }
        [HttpGet("api/pay")]
        //public async Task<PayRootCollectionJson> Get()
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _payRepo.GetPays();
                if (results.PayJson.Any())
                {
                    return Ok(results);
                }
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = "Sequnce contains no elements." } });
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }
        [HttpGet("api/pay/sitter-id/{id:int}")]
        //public async Task<PayRootCollectionJson> GetPaySitterId(int id)
        public async Task<IActionResult> GetPaySitterId(int id)
        {
            try
            {
                var results = await _payRepo.GetPays(id);
                if (results.PayJson.Any())
                {
                    return Ok(results);
                }
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = "Sequence contains no elements." } });
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }

        [HttpGet("api/pay/sitter-id/{id:int}/{date:datetime}")]
        //public async Task<PayRootCollectionJson> GetPaySitterId(int id, DateTime date)
        public async Task<IActionResult> GetPaySitterId(int id, DateTime date)
        {
            try
            {
                var results = await _payRepo.GetPays(id, date);
                if (results.PayJson.Any())
                {
                    return Ok(results);
                }
                return Ok(new PayRootCollectionJson() { PayJson = new List<PayJson>() { new PayJson() { Comment = "Sequence contains no elements." } } });
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }
        // GET api/<controller>/5
        [HttpGet("api/pay/{id:int}")]
        //public PayRootJson Get(int id)
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(_payRepo.GetPayString(id));
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }
        [HttpPost("api/pay")]
        public HttpResponseMessage Post([FromBody]PayRoot value)
        {
            var responseError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK);
            var authKey = getAuthKey(Request.Method);
            if (isAuthorized())
            {
                responseError.ReasonPhrase = "Missing API-Authorization Header";
                return responseError;
            }
            else if (!isAuthorizedMatched(authKey))
            {
                responseError.ReasonPhrase = "Invalid API-Authorization Header";
                return responseError;
            }
            if (value == null)
            {
                responseError.ReasonPhrase = "Null Response Received";
                return responseError;
            }
            var pay = value.Pay;
            var x = pay.CheckDate.CompareTo(DateTime.MinValue);
            var y = new DateTime(2001, 1, 1, 0, 0, 0);
            if (pay.SitterId <= 0 ||
                pay.CheckDate.CompareTo(DateTime.MinValue) == 0)
            {
                responseError.ReasonPhrase = "timePay is zero or null or sitterId is null or zero";
                return responseError;
            }
            try
            {
                var result = _payRepo.AddPay(pay);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Pay {pay.Id} not updated";
                    return responseError;
                }
            }
            catch (Exception e)
            {
                responseError.ReasonPhrase = e.Message;
                return responseError;
            }
            responseOk.ReasonPhrase = $"Pay Added";
            return responseOk;
        }
        [HttpPut("api/pay")]
        public HttpResponseMessage Put([FromBody]PayRoot value)
        {
            var responseError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK);
            if (!ModelState.IsValid)
            {
                responseError.ReasonPhrase = "Invalid Model State";
                return responseError;
            }
            String authKey = getAuthKey(Request.Method);
            if (isAuthorized())
            {
                responseError.ReasonPhrase = "Missing API-Authorization Header";
                return responseError;
            }
            else if (!isAuthorizedMatched(authKey))
            {
                responseError.ReasonPhrase = "Invalid API-Authorization Header";
                return responseError;
            }
            if (value == null)
            {
                responseError.ReasonPhrase = "Null Response Received.";
                return responseError;
            }
            var pay = value.Pay;
            if (pay.Id == 0)
            {
                responseError.ReasonPhrase = "Pay id is zero";
                return responseError;
            }
            else
            {
                var result = _payRepo.UpdatePay(pay);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Pay id {pay.Id} not updated.";
                    return responseError;
                }
                responseOk.ReasonPhrase = $"Pay id {pay.Id} was updated.";
                return responseOk;
            }
        }
        [HttpDelete("api/pay/{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            var responseError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK);
            String authKey = getAuthKey(Request.Method);
            if (isAuthorized())
            {
                responseError.ReasonPhrase = "Missing API-Authorization Header";
                return responseError;
            }
            else if (!isAuthorizedMatched(authKey))
            {
                responseError.ReasonPhrase = "Invalid API-Authorization Header";
                return responseError;
            }
            if (id == 0)
            {
                responseError.ReasonPhrase = "Pay id is zero";
                return responseError;
            }
            try
            {
                var result = _payRepo.DeletePay(id);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Pay id {id} not deleted";
                    return responseError;
                }
            }
            catch (Exception e)
            {
                responseError.ReasonPhrase = e.Message;
                return responseError;
            }
            responseOk.ReasonPhrase = $"Pay id {id} was deleted";
            return responseOk;
        }
        bool isAuthorized()
        {
            return String.IsNullOrEmpty(Request.Headers["API-Authorization"]);
        }
        bool isAuthorizedMatched(string authKey)
        {
            return Request.Headers["API-Authorization"].Equals(authKey);
        }
        String getAuthKey(string method)
        {
            try
            {
                var authRoot = _authRepo.GetAuth(method.ToUpper());
                return authRoot.Auth.AuthKey;
            }
            catch (Exception)
            {
                switch (method)
                {
                    case "GET":
                        return _getAuthKey;
                    case "POST":
                        return _postAuthKey;
                    case "PUT":
                        return _putAuthKey;
                    case "DELETE":
                        return _deleteAuthKey;
                }
            }
            return null;
        }
    }
}