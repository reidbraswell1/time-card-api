using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TimeCard.Shared;
namespace TimeCard.Controllers
{
    //[Route("api/sitter")]
    public class SitterApiController : Controller
    {

        // For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
        private readonly ISitterRepository _sitterRepo;
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _configuration;
        private readonly String _getAuthKey;
        private readonly String _postAuthKey;
        private readonly String _putAuthKey;
        private readonly String _deleteAuthKey;

        public SitterApiController(ISitterRepository sitterRepo, IAuthRepository authRepo, IConfiguration configuration)
        {
            _sitterRepo = sitterRepo;
            _authRepo = authRepo;
            _configuration = configuration;
            _getAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Post").Value;
            _postAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Post").Value;
            _putAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Put").Value;
            _deleteAuthKey = _configuration.GetSection("APIKeys").GetSection("API-Authorization-Delete").Value;
        }

        // GET: api/<controller>
        [HttpGet("api/sitter")]
        public async Task<SittersRootCollectionJson> Get()
        {
            try
            {
                return await _sitterRepo.GetSitters();
            }
            catch (Exception e)
            {
                return new SittersRootCollectionJson();
            }
        }

        // GET api/<controller>/5
        [HttpGet("api/sitter/{id:int}")]
        public SittersRoot Get(int id)
        {
            try
            {
                return _sitterRepo.GetSitter(id);
            }
            catch (Exception e)
            {
                return new SittersRoot();
            }
        }
        // POST api/<controller>
        [HttpPost("api/sitter")]
        public HttpResponseMessage Post([FromBody]SittersRoot value)
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
            var sitter = value.Sitter;
            if (string.IsNullOrEmpty(sitter.LastName) ||
                string.IsNullOrEmpty(sitter.FirstName) ||
                sitter.RateId <= 0)
            {
                responseError.ReasonPhrase = "Sitter id is zero or null or empty lastname, firstname or zero rateId";
                return responseError;
            }
            try
            {
                var result = _sitterRepo.AddSitter(sitter);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Sitter {sitter.Id} not updated";
                    return responseError;
                }
            }
            catch (Exception e)
            {
                responseError.ReasonPhrase = e.Message;
                return responseError;
            }
            responseOk.ReasonPhrase = "Sitter Added";
            return responseOk;
        }
        [HttpPut("api/sitter")]
        public HttpResponseMessage Put([FromBody]SittersRoot value)
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
            if (value == null)
            {
                responseError.ReasonPhrase = "Null Response Received.";
                return responseError;
            }
            var sitter = value.Sitter;
            if (sitter.Id == 0)
            {
                responseError.ReasonPhrase = "Sitter id is zero";
                return responseError;
            }
            else
            {
                var result = _sitterRepo.UpdateSitter(sitter);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Sitter id {sitter.Id} not updated.";
                    return responseError;
                }
                responseOk.ReasonPhrase = $"Sitter id {sitter.Id} was updated.";
                return responseOk;
            }
        }
        [HttpDelete("api/sitter/{id:int}")]
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
                responseError.ReasonPhrase = "Sitter id is zero";
                return responseError;
            }
            try
            {
                var result = _sitterRepo.DeleteSitter(id);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Sitter id {id} not deleted";
                    return responseError;
                }
            }
            catch (Exception e)
            {
                responseError.ReasonPhrase = e.Message;
                return responseError;
            }
            responseOk.ReasonPhrase = $"Sitter id {id} was deleted";
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