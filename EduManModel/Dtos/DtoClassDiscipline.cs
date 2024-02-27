using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoClassDiscipline
	{
		public DtoClassDiscipline()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "int", "int", "int", "date", "int" };
		}
		public DtoClassDiscipline(int? id, int? classdistributeid, int? disciplineid, int? weeklyid, DateTime? ondate, int? times)
		{
			Id = id;
			ClassDistributeId = classdistributeid;
			DisciplineId = disciplineid;
			WeeklyId = weeklyid;
			OnDate = ondate;
			Times = times;
			TypeList = new(){ "int", "int", "int", "int", "date", "int" };
		}
		public int? Id { get; set; }
		public int? ClassDistributeId { get; set; }
		public int? DisciplineId { get; set; }
		public int? WeeklyId { get; set; }
		public DateTime? OnDate { get; set; }
		public int? Times { get; set; }
		public List<string> TypeList { get; set; }
	}
}
