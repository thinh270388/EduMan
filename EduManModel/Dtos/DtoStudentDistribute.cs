using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoStudentDistribute
	{
		public DtoStudentDistribute()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "int", "int", "date", "nvarchar" };
		}
		public DtoStudentDistribute(int? id, int? classdistributeid, int? studentid, DateTime? assigndate, string? note)
		{
			Id = id;
			ClassDistributeId = classdistributeid;
			StudentId = studentid;
			AssignDate = assigndate;
			Note = note;
			TypeList = new(){ "int", "int", "int", "date", "nvarchar" };
		}
		public int? Id { get; set; }
		public int? ClassDistributeId { get; set; }
		public int? StudentId { get; set; }
		public DateTime? AssignDate { get; set; }
		public string? Note { get; set; }
		public List<string> TypeList { get; set; }
	}
}
