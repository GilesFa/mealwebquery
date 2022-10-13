using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrjMealWebQuery.Models
{
    public class DBContext:DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet<BLunchMain> BLunchMains { get; set; }
        public DbSet<BLunchDetail> BLunchDetails { get; set; }
        public DbSet<UmHrEmp> UmHrEmps { get; set; }
        public DbSet<UmCardMap> UmCardMaps { get; set; }
        public DbSet<MealMain> MealMains { get; set; }
        public DbSet<MealDetail> MealDetails { get; set; }

        public DbSet<UmHrMealShift> UmHrMealShifts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BLunchMain>().HasKey(t => new { t.MEAL_DATE });
            modelBuilder.Entity<BLunchDetail>().HasKey(t => new { t.MEAL_DATE,t.EMP_NO });
            modelBuilder.Entity<UmHrEmp>().HasKey(t=>new { t.EMP_NO});
            modelBuilder.Entity<UmCardMap>().HasKey(t => new {t.STFNO,t.CARDNO,t.CARDSNSH,t.CARDSNTH });
            modelBuilder.Entity<MealMain>().HasKey(t => new { t.EAT_YEAR, t.EAT_MONTH });
            modelBuilder.Entity<MealDetail>().HasKey(t => new { t.EMP_NO,t.EAT_YEAR, t.EAT_MONTH });
            modelBuilder.Entity<UmHrMealShift>().HasKey(t => new { t.DEPT });
            base.OnModelCreating(modelBuilder);
        }
    }

    [Table("MEAL_MAIN", Schema = "UMEAT")]
    public class MealMain {
        public string EAT_YEAR { get; set; }
        public string EAT_MONTH { get; set; }
        public string START_BOOK_DATETIME { get; set; }
        public string END_BOOK_DATETIME { get; set; }
        public string CRT_DATE { get; set; }
        public string CRT_USER { get; set; }
        public string UPD_DATE { get; set; }
        public string UPD_TIME { get; set; }
        public string UPD_USER { get; set; }
    }

    [Table("MEAL_DETAIL", Schema = "UMEAT")]
    public class MealDetail {
        public string EMP_NO { get; set; }
        public string EAT_YEAR { get; set; }
        public string EAT_MONTH { get; set; }
        public int BREAKFAST { get; set; }
        public int LUNCH { get; set; }
        public int BOXED_LUNCH { get; set; }
        public int BOXED_VEGETARIAN_LUNCH { get; set; }
        public int DINNER { get; set; }
        public int BOXED_DINNER { get; set; }
        public int BOXED_VEGETARIAN_DINNER { get; set; }
        public string CRT_DATE { get; set; }
        public string CRT_USER { get; set; }
        public string UPD_DATE { get; set; }
        public string UPD_TIME { get; set; }
        public string UPD_USER { get; set; }

    }

    


    [Table("B_LUNCH_MAIN",Schema ="UMEAT")]
    public class BLunchMain {
        public string MEAL_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public string START_BOOK_DATETIME { get; set; }
        public string END_BOOK_DATETIME { get; set; }
        public int MAX_NUMBER { get; set; }
        public string CRT_DATE { get; set; }
        public string CRT_USER { get; set; }
        public string UPD_DATE { get; set; }
        public string UPD_TIME { get; set; }
        public string UPD_USER { get; set; }

    }

    [Table("B_LUNCH_DETAIL", Schema = "UMEAT")]
    public class BLunchDetail {
        public string MEAL_DATE { get; set; }
        public string EMP_NO { get; set; }
        public string ENABLE_FLAG { get; set; }
        public string IsTaked { get; set; }
        public string programId { get; set; }
        public string processIpAddress { get; set; }
        public string CRT_DATE { get; set; }
        public string CRT_USER { get; set; }
        public string UPD_DATE { get; set; }
        public string UPD_TIME { get; set; }
        public string UPD_USER { get; set; }

    }
    [Table("UM_CARD_MAP",Schema ="UMEAT")]
    public class UmCardMap {
        public string STFNO { get; set; }
        public string CARDNO { get; set; }
        public string CARDSNSH { get; set; }
        public string CARDSNTH { get; set; }
        public string CHANGTM { get; set; }
    }

    [Table("UM_HR_EMP",Schema ="UMEAT")]
    public class UmHrEmp
    {
        public string EMP_NO { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_NAME_ENG { get; set; }
        public string SEX_M_OR_F { get; set; }
        public string AREA_FLAG { get; set; }
        public string AD_ACCOUNT { get; set; }
        public string EMAIL { get; set; }
        public int TITLE_DEGREE { get; set; }
        public string TITLE_NAME { get; set; }
        public string DEPT_ID { get; set; }
        public string DEPT_NAME { get; set; }
        public string INDATE { get; set; }
        public string OUTDATE { get; set; }
        public string ENABLE_FLAG { get; set; }
        public string JOB { get; set; }
        public string CATEGORY { get; set; }
        public string EMP_TYPE { get; set; }
    }

    [Table("UM_HR_MEAL_SHIFT", Schema = "UMEAT")]
    public class UmHrMealShift
    {
        public string DEPT { get; set; }
        public string BEGIN_TIME { get; set; }
        public string END_TIME { get; set; }
    }

}
