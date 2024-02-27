using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoLevel
	{
		public DtoLevel()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar" };
		}
		public DtoLevel(int? id, string? levelname)
		{
			Id = id;
			LevelName = levelname;
			TypeList = new(){ "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? LevelName { get; set; }
		public List<string> TypeList { get; set; }
	}
}
