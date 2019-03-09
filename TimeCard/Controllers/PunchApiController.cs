using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeCard.Shared;
namespace TimeCard.Controllers
{
    //[Route("api/punch")]
    public class PunchApiController : Controller
    {

        // For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
        private readonly IPunchRepository _punchRepo;
        public PunchApiController(IPunchRepository punchRepo)
        {
            _punchRepo = punchRepo;
        }
        // GET: api/<controller>
        [HttpGet("api/punch")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var results = await _punchRepo.GetPunches();
                if (results.Punches.Any())
                {
                    return Ok(results);
                }
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = "Sequence contains no elements" } });
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }
        [HttpGet("api/punch/sitter-id/{id:int}")]
        //public async Task<PunchRootCollection> GetPunchSitterId(int id)
        public async Task<IActionResult> GetPunchSitterId(int id)
        {
            try
            {
                var results = await _punchRepo.GetPunches(id);
                if (results.PunchJson.Any())
                    return Ok(results);
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = "Sequence contains no elements" } });
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }
        [HttpGet("api/punch/sitter-id/{id:int}/{periodStart:datetime}/{periodEnd:datetime}")]
        //public async Task<PunchRootCollectionJson> GetPunchSitterId(int id, DateTime periodStart, DateTime periodEnd)
        public async Task<IActionResult> GetPunchSitterId(int id, DateTime periodStart, DateTime periodEnd)
        {
            try
            {
                var results = await _punchRepo.GetPunches(id, periodStart, periodEnd);
                if (results.PunchJson.Any())
                {
                    return Ok(results);
                }
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = "Sequence contains no elements" } });
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }

        [HttpGet("api/punch/sitter-id-list/{id:int}/{periodStart:datetime}/{periodEnd:datetime}")]
        public async Task<List<IGrouping<String, String>>> GetPunchSitterIdList(int id, DateTime periodStart, DateTime periodEnd)
        {
            try
            {
                var results = await _punchRepo.GetPunches(id, periodStart, periodEnd);
                if (results.PunchJson.Any())
                {
                    var punches = new List<String>();
                    var punches2 = new List<String>();
                    var punches4 = new List<IGrouping<String, String>>();
                    var sumTime = 0.0;
                    var even = true;
                    foreach (var punchJson in results.PunchJson)
                    {
                        if (even)
                        {
                            sumTime += Double.Parse(punchJson.TimePunchSeconds);
                            even = false;
                        }
                        else
                        {
                            sumTime += Double.Parse(punchJson.TimePunchSeconds) * -1;
                            even = true;
                        }
                        punches.Add(punchJson.SitterId + " " + punchJson.TimePunch);
                        var punches3 = from punch in punches
                                       group punch by punch.Substring(2, 3);
                        punches4 = punches3.ToList();
                    }
                    punches.Add("Total Time = " + Math.Abs(sumTime / 3600));
                    return punches4;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("api/punch/{id:int}")]
        //public PunchRootJson Get(int id)
        public IActionResult Get(int id)
        {
            try
            {
                var results = _punchRepo.GetPunch(id);
                if (results != null)
                {
                    return Ok(results);
                }
                else
                {
                    return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = "Sequence contains no elements" } });
                }
            }
            catch (Exception e)
            {
                return Ok(new ErrorException() { ErrorExceptionMessage = new ErrorExceptionMessage() { Message = e.Message } });
            }
        }
        // POST api/<controller>
        [HttpPost("api/punch")]
        public HttpResponseMessage Post([FromBody]PunchRoot value)
        {
            var responseError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK);
            if (!ModelState.IsValid)
            {
                responseError.ReasonPhrase = "Model State is Invalid";
                return responseError;
            }
            if (value == null)
            {
                responseError.ReasonPhrase = "Null Response Received";
                return responseError;
            }
            var punch = value.Punch;
            var x = punch.TimePunch.CompareTo(DateTime.MinValue);
            var y = new DateTime(2001, 1, 1, 0, 0, 0);
            if (punch.SitterId <= 0 ||
                punch.TimePunch.CompareTo(DateTime.MinValue) == 0)
            {
                responseError.ReasonPhrase = "timePunch is zero or null or sitterId is null or zero";
                return responseError;
            }
            try
            {
                var result = _punchRepo.AddPunch(punch);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Punch {punch.Id} not updated";
                    return responseError;
                }
            }
            catch (Exception e)
            {
                responseError.ReasonPhrase = e.Message;
                return responseError;
            }
            responseOk.ReasonPhrase = $"Punch Added";
            return responseOk;
        }

        // PUT api/<controller>/5
        [Route("api/punch")]
        [HttpPut]
        public HttpResponseMessage Put([FromBody]PunchRoot value)
        {
            var responseError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK);
            if (!ModelState.IsValid)
            {
                responseError.ReasonPhrase = "Model State is Invalid";
                return responseError;
            }
            if (value == null)
            {
                responseError.ReasonPhrase = "Null Response Received.";
                return responseError;
            }
            var punch = value.Punch;
            if (punch.Id == 0)
            {
                responseError.ReasonPhrase = "Punch id is zero";
                return responseError;
            }
            else
            {
                var result = _punchRepo.UpdatePunch(punch);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Punch id {punch.Id} not updated.";
                    return responseError;
                }
                responseOk.ReasonPhrase = $"Punch id {punch.Id} was updated.";
                return responseOk;
            }
        }

        // DELETE api/<controller>/5
        [Route("api/punch/{id:int}")]
        [HttpDelete("{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            var responseError = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var responseOk = new HttpResponseMessage(HttpStatusCode.OK);
            if (id == 0)
            {
                responseError.ReasonPhrase = "Punch id is zero";
                return responseError;
            }
            try
            {
                var result = _punchRepo.DeletePunch(id);
                if (result == 0)
                {
                    responseError.ReasonPhrase = $"Punch id {id} not deleted";
                    return responseError;
                }
            }
            catch (Exception e)
            {
                responseError.ReasonPhrase = e.Message;
                return responseError;
            }
            responseOk.ReasonPhrase = $"Punch id {id} was deleted";
            return responseOk;
        }
    }
}