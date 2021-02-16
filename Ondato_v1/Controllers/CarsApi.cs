using Ondato.Model;
using Ondato.Model.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ondato.Attributes;

namespace Ondato.Controllers
{
    [ApiController]
    [Route("api/[action]")]
    [ApiKey]
    public class WordApi : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly CarDataHelper _helper;

        public WordApi(IConfiguration iConfig)
        {
            Configuration = iConfig;
            _helper = new CarDataHelper(iConfig);
        }

        //[HttpGet]
        //[ActionName("WordCount")]
        //[ResponseCache(Duration = 360, VaryByQueryKeys = new[] { "sentece" })]


        [HttpPut]
        [ActionName("CarsInsert")]
        //[ResponseCache(Duration = 360)]
        public IActionResult Insert([FromBody] CarsByMaker makerCars, int? daysValid)
        {
            try
            {
                string Resp = "";
                if (_helper.CheckExistence(makerCars.Maker) == true)
                    Resp = _helper.Update(makerCars, daysValid);
                else
                    Resp = _helper.AddNew(makerCars, daysValid);
                return Ok(Resp);
            }
            catch (Exception ex)
            {
                // LogHelper.LogErrorMessage(ex);
                return BadRequest("Undefined error. Contact dev.");
            }
        }
        [HttpGet]
        [ActionName("CarsGet")]
        //[ResponseCache(Duration = 360)]
        public IActionResult Get(string manufacturer)
        {
            try
            {
                return Ok(_helper.GetByKey(manufacturer));
            }
            catch (Exception ex)
            {
                // LogHelper.LogErrorMessage(ex);
                return BadRequest("Undefined error. Contact dev.");
            }
        }
        [HttpDelete]
        [ActionName("CarsDelete")]
        //[ResponseCache(Duration = 360)]
        public IActionResult Delete(string manufacturer)
        {
            try
            {
                return Ok(_helper.DeleteByKey(manufacturer));
            }
            catch (Exception ex)
            {
                // LogHelper.LogErrorMessage(ex);
                return BadRequest("Undefined error. Contact dev.");
            }
        }
    }
}
