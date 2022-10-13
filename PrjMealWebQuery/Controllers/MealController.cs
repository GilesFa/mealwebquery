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
    public class MealController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;//這很像ASP.net的WebConfigurationManager
        private readonly DBContext eatDBContext;
        private readonly OracleConnection oraConn;
        private readonly IHostEnvironment _env;

        public MealController(ILogger<HomeController> logger, IConfiguration config, DBContext dbContext, IHostEnvironment env, OracleConnection oraConn)
        {
            this._logger = logger;
            this._config = config;
            this.eatDBContext = dbContext;
            this._env = env;
            this.oraConn = oraConn;
        }

        public IActionResult Index()
        {
            string eatYear = DateTime.Now.ToString("yyyy");
            string eatMonth = DateTime.Now.ToString("MM");
            ViewBag.EatYear = eatYear;
            ViewBag.EatMonth = eatMonth;
            return View();
        }

        public ContentResult GetMealDetail(string empNO) {
            string eatYear = DateTime.Now.ToString("yyyy");
            string eatMonth = DateTime.Now.ToString("MM");

            string result = "[]";
            using (var eatDb = this.eatDBContext)
            {
                var output = from detail in eatDb.MealDetails
                             where (eatYear.Equals(detail.EAT_YEAR) 
                             && eatMonth.Equals(detail.EAT_MONTH)
                             && empNO.Equals(detail.EMP_NO))
                             select detail;
                if (output.Count() > 0) {
                    result = JsonConvert.SerializeObject(output);
                }
            }
            
            return Content(result,"application/json");
        }

        public ContentResult GetEmpData(string empNo) {
            string result = "[]";
            using (var eatDb = this.eatDBContext)
            {
                var output = from detail in eatDb.UmHrEmps
                             where empNo.Equals(detail.EMP_NO)
                             select detail;
                if (output.Count() > 0)
                {
                    result = JsonConvert.SerializeObject(output);
                }
            }
            return Content(result, "application/json");

        }

        public ContentResult GetEmpNoFromERP(string CARDSNTH)
        {

            int cardNoSNTH = 0;
            bool isNumber = int.TryParse(CARDSNTH, out cardNoSNTH);

            if (!isNumber)
            {
                return Content("{error:'參數有誤'}", "application/json");
            }

            using (var db = this.eatDBContext)
            {
                var emp = from card in db.UmCardMaps
                          join hr in db.UmHrEmps on card.STFNO.Substring(1, 5) equals hr.EMP_NO.Substring(2, 5)
                          where card.CARDSNTH.Equals(cardNoSNTH.ToString())
                          && hr.AREA_FLAG.Equals("TW")
                          && hr.ENABLE_FLAG.Equals("Y")
                          select new { hr.EMP_NO, card.CARDNO, card.CARDSNSH, card.CARDSNTH, hr.EMP_NAME, hr.AREA_FLAG, hr.DEPT_ID, hr.DEPT_NAME, card.CHANGTM };

                if (emp.Count() == 1)
                {
                    return Content(JsonConvert.SerializeObject(emp), "application/json");
                }
                else if (emp.Count() > 1)
                {
                    return Content("{error:'卡號有問題'}", "application/json");
                }
            }

            return Content("{error:'無此卡號'}", "application/json");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
