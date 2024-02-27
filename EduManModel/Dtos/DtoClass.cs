using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoClass
	{
		public DtoClass()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar", "int" };
		}
		public DtoClass(int? id, string? classname, int? gradeid)
		{
			Id = id;
			ClassName = classname;
			GradeId = gradeid;
			TypeList = new(){ "int", "nvarchar", "int" };
		}
		public int? Id { get; set; }
		public string? ClassName { get; set; }
		public int? GradeId { get; set; }
		public List<string> TypeList { get; set; }
	}
}
