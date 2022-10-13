using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PrjMealWebQuery.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using IO = System.IO;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace PrjMealWebQuery.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;//這很像ASP.net的WebConfigurationManager
        private readonly DBContext eatDBContext;
        private readonly OracleConnection oraConn;
        private readonly IHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, DBContext dbContext, IHostEnvironment env, OracleConnection oraConn)
        {
            this._logger = logger;
            this._config = config;
            this.eatDBContext = dbContext;
            this._env = env;
            this.oraConn = oraConn;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
