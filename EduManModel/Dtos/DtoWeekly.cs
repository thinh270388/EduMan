using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoWeekly
	{
		public DtoWeekly()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "int", "nvarchar", "date", "date", "int", "int", "int", "nvarchar", "nvarchar", "nvarchar" };
		}
		public DtoWeekly(int? id, int? startweekid, string? weeklyname, DateTime? fromdate, DateTime? todate, int? numberoflession, int? initialpoint, int? coefficient, string? ondutyclass, string? sumarizing, string? planning)
		{
			Id = id;
			StartWeekId = startweekid;
			WeeklyName = weeklyname;
			FromDate = fromdate;
			ToDate = todate;
			NumberOfLession = numberoflession;
			InitialPoint = initialpoint;
			Coefficient = coefficient;
			OnDutyClass = ondutyclass;
			Sumarizing = sumarizing;
			Planning = planning;
			TypeList = new(){ "int", "int", "nvarchar", "date", "date", "int", "int", "int", "nvarchar", "nvarchar", "nvarchar" };
		}
		public int? Id { get; set; }
		public int? StartWeekId { get; set; }
		public string? WeeklyName { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }
		public int? NumberOfLession { get; set; }
		public int? InitialPoint { get; set; }
		public int? Coefficient { get; set; }
		public string? OnDutyClass { get; set; }
		public string? Sumarizing { get; set; }
		public string? Planning { get; set; }
		public List<string> TypeList { get; set; }
	}
}
