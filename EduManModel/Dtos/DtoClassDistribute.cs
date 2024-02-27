using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoClassDistribute
	{
		public DtoClassDistribute()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "int", "int", "date", "varchar", "nvarchar" };
		}
		public DtoClassDistribute(int? id, int? classid, int? teacherid, DateTime? assigndate, string? onyear, string? note)
		{
			Id = id;
			ClassId = classid;
			TeacherId = teacherid;
			AssignDate = assigndate;
			OnYear = onyear;
			Note = note;
			TypeList = new(){ "int", "int", "int", "date", "varchar", "nvarchar" };
		}
		public int? Id { get; set; }
		public int? ClassId { get; set; }
		public int? TeacherId { get; set; }
		public DateTime? AssignDate { get; set; }
		public string? OnYear { get; set; }
		public string? Note { get; set; }
		public List<string> TypeList { get; set; }
	}
}
