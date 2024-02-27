using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoFunct
	{
		public DtoFunct()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar" };
		}
		public DtoFunct(int? id, string? functname)
		{
			Id = id;
			FunctName = functname;
			TypeList = new(){ "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? FunctName { get; set; }
		public List<string> TypeList { get; set; }
	}
}
