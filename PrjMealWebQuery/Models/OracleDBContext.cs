using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrjMealWebQuery.Models
{
	public class OracleDBContext : DbContext
	{
		public OracleDBContext(DbContextOptions<OracleDBContext> options) : base(options) { }
		
		public DbSet<QryHrCard> QRY_HR_CARDs { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}

	public class QryHrCard
	{
		string EMP_NO { get; set; }
		string CARDNO{get;set;}
	    string CARDSNSH{get;set;}
		long CARDSNTH { get; set; }
		string EMP_NAME { get; set; }
		string AREA_FLAG { get; set; }
		string DEPT_ID { get; set; }
		string DEPT_NAME { get; set; }
		string CHANGTM { get; set; }
	}
	
}