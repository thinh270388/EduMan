using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoStartWeek
	{
		public DtoStartWeek()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "varchar", "date", "bit" };
		}
		public DtoStartWeek(int? id, string? onyear, DateTime? startdate, bool? used)
		{
			Id = id;
			OnYear = onyear;
			StartDate = startdate;
			Used = used;
			TypeList = new(){ "int", "varchar", "date", "bit" };
		}
		public int? Id { get; set; }
		public string? OnYear { get; set; }
		public DateTime? StartDate { get; set; }
		public bool? Used { get; set; }
		public List<string> TypeList { get; set; }
	}
}
