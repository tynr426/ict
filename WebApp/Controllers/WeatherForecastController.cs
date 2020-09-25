using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.BLL;
using ECF;
using ECF.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDapperFactory _dapperFactory;
        public WeatherForecastController(IDapperFactory dapperFactory)
        {
            _dapperFactory = dapperFactory;
        }

        //public WeatherForecastController(ILogger<WeatherForecastController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet]
        public string Get()
        {
           return new ConnectorBusiness(_dapperFactory).DBTable().ToJson();
        }
    }
}
