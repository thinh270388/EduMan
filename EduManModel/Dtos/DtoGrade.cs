using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoGrade
	{
		public DtoGrade()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar", "int" };
		}
		public DtoGrade(int? id, string? gradename, int? levelid)
		{
			Id = id;
			GradeName = gradename;
			LevelId = levelid;
			TypeList = new(){ "int", "nvarchar", "int" };
		}
		public int? Id { get; set; }
		public string? GradeName { get; set; }
		public int? LevelId { get; set; }
		public List<string> TypeList { get; set; }
	}
}
