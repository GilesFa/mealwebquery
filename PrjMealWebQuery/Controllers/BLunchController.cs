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
    public class BLunchController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;//這很像ASP.net的WebConfigurationManager
        private readonly DBContext eatDBContext;
        private readonly OracleConnection oraConn;
        private readonly IHostEnvironment _env;
        public BLunchController(ILogger<HomeController> logger, IConfiguration config, DBContext dbContext, IHostEnvironment env, OracleConnection oraConn) {
            this._logger = logger;
            this._config = config;
            this.eatDBContext = dbContext;
            this._env = env;
            this.oraConn = oraConn;

        }
        public IActionResult Index()
        {
            _logger.LogInformation("aaa information");


            string currentDate = DateTime.Now.ToString("yyyy/MM/dd");
            string outdateLimit = DateTime.Now.AddMonths(-6).ToString("yyyy/MM/dd");
            string bLunchMain = "[]";
            using (var umeatDb = this.eatDBContext)
            {
                var output = umeatDb.BLunchMains
                                     .Where(o => (currentDate.CompareTo(o.MEAL_DATE) <= 0));
                bLunchMain = JsonConvert.SerializeObject(output);
            }
            ViewBag.BLunchMains = bLunchMain;
            return View();
        }

        /// <summary>
        /// 撈取所有ERP的員工卡片資料
        /// 目前沒有用到
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private string GetOracleEmplData()
        {
            string[] path = { this._env.ContentRootPath, "wwwroot", "sql", "QRY_HR_CARD_ALL.sql" };
            var pathToFile = IO.Path.Combine(path);
            var oraSql = IO.File.ReadAllText(pathToFile);
            string oracleEmpl = "[]";
            DataTable dt = new DataTable();
            DateTime bgnTime = DateTime.Now;

            using (var conn = oraConn)
            {
                conn.Close();
                OracleCommand comm = new OracleCommand(oraSql, conn);
                conn.Open();
                var dr = comm.ExecuteReader();
                dt.Load(dr);
                conn.Close();
            };

            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - bgnTime;
            _logger.LogInformation("GetOracleEmplData 花了" + ts.ToString());


            if (dt.Rows.Count > 0)
            {
                oracleEmpl = JsonConvert.SerializeObject(dt);
            }
            return oracleEmpl;
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

        public void SetBLunchDataIsTakedByEmpNo(string MEAL_DATE, string EMP_NO)
        {

            DateTime current = DateTime.Now;
            string IsTaked = "Y";
            string programId = "HomeController.SetBLunchDataIsTakedByEmpNo";
            string updDate = current.ToString("yyyy/MM/dd"); // case sensitive;
            string updTime = current.ToString("HH/mm/ss"); // case sensitive;
            string ipAddress = this.HttpContext.Connection.RemoteIpAddress.ToString();

            using (var umeatDb = this.eatDBContext)
            {
                var output = from bLunchDetails in umeatDb.BLunchDetails
                             where (bLunchDetails.MEAL_DATE.Equals(MEAL_DATE)
                             && bLunchDetails.EMP_NO.Equals(EMP_NO)
                             && bLunchDetails.ENABLE_FLAG.Equals("Y"))
                             select bLunchDetails;

                if (output.Count() == 1)
                {
                    var bdl = output.FirstOrDefault();

                    if ("N".Equals(bdl.IsTaked))
                    {
                        bdl.IsTaked = IsTaked;
                        bdl.programId = programId;
                        bdl.UPD_DATE = updDate;
                        bdl.UPD_TIME = updTime;
                        bdl.UPD_USER = EMP_NO;
                        bdl.processIpAddress = ipAddress;
                        umeatDb.SaveChangesAsync();
                    }
                }
            }
        }

        public ActionResult GetBLunchStatistic(string MEAL_DATE)
        {
            try
            {
                if (MEAL_DATE == null)
                {
                    throw new Exception("缺少參數 MEAL_DATE");
                }

                string BLunchStatisticData = null;
                int resultCount = 0;
                using (var umeatDb = this.eatDBContext)
                {
                    // 已領人數
                    /*
                    var output = from bStatData in umeatDb.BLunchDetails
                                 where (bStatData.MEAL_DATE.Equals(MEAL_DATE) &&
                                        "Y".Equals(bStatData.ENABLE_FLAG) &&
                                        "Y".Equals(bStatData.IsTaked)
                                        )
                                 group bStatData by new { bStatData.IsTaked } into g
                                 select new { IsTaked = g.Key.IsTaked, Quantity = g.Count() };
                    */
                    

                    var detail = from bStatData in umeatDb.BLunchDetails
                                 join hr in umeatDb.UmHrEmps on bStatData.EMP_NO equals hr.EMP_NO
                                 join mealShift in umeatDb.UmHrMealShifts on hr.DEPT_ID.Substring(1,4) equals mealShift.DEPT into ps
                                 from o in ps.DefaultIfEmpty()
                                 where (bStatData.MEAL_DATE.Equals(MEAL_DATE) &&
                                        "Y".Equals(bStatData.ENABLE_FLAG) 
                                        )
                                 select new { bStatData.EMP_NO,
                                     bStatData.IsTaked,
                                     BEGIN_TIME = o.BEGIN_TIME??"ELSE"
                                 };

                    var result = from a in detail
                              group a by new {a.IsTaked,a.BEGIN_TIME } into g
                              select new { IsTaked = g.Key.IsTaked,g.Key.BEGIN_TIME, Quantity = g.Count() };

                    BLunchStatisticData = JsonConvert.SerializeObject(result);
                    resultCount = result.Count();
                }

                if (BLunchStatisticData != null && resultCount > 0)
                {
                    return Content(BLunchStatisticData, "application/json");
                }

                var errorOutput = new List<Object>();
                errorOutput.Add(new { IsTaked = "Y", Quantity = 0 });
                errorOutput.Add(new { IsTaked = "N", Quantity = 0 });
                string errorString = JsonConvert.SerializeObject(errorOutput);
                return Content(errorString, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }



        }

        public ContentResult GetBLunchDataByEmpNo(string MEAL_DATE, string EMP_NO)
        {
            string BLunchDetail = "[]";
            DateTime bgnTime = DateTime.Now;
            try
            {
                using (var umeatDb = this.eatDBContext)
                {
                    var output = from bLunchDetails in umeatDb.BLunchDetails
                                 where (bLunchDetails.MEAL_DATE.Equals(MEAL_DATE)
                                 && bLunchDetails.EMP_NO.Equals(EMP_NO)
                                 && bLunchDetails.ENABLE_FLAG.Equals("Y"))
                                 select bLunchDetails;
                    BLunchDetail = JsonConvert.SerializeObject(output);
                }
            }
            catch (Exception ex)
            {
                //throw;
                return Content("{error:" + "'" + ex.Message + "'}", "application/json");
            }

            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - bgnTime;
            _logger.LogInformation("GetBLunchDataByEmpNo 花了" + ts.ToString());
            return Content(BLunchDetail, "application/json");
        }

        public ContentResult GetBLunchData(string bLunchMealDate)
        {
            string bLunchDetail = "[]";
            DateTime bgnTime = DateTime.Now;
            using (var umeatDb = this.eatDBContext)
            {
                var output = from bLunchDetails in umeatDb.BLunchDetails
                             where bLunchDetails.MEAL_DATE.Equals(bLunchMealDate)
                             orderby bLunchDetails.EMP_NO
                             select new
                             {
                                 empNo = bLunchDetails.EMP_NO,
                                 mealDate = bLunchDetails.MEAL_DATE,
                                 updDate = bLunchDetails.UPD_DATE,
                                 updTime = bLunchDetails.UPD_TIME,
                                 updUser = bLunchDetails.UPD_USER
                             };

                bLunchDetail = JsonConvert.SerializeObject(output);

            }
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - bgnTime;
            _logger.LogInformation("GetBLunchDataByEmpNo 花了" + ts.ToString());

            return Content(bLunchDetail, "application/json");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
