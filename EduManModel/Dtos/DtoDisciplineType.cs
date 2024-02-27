using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoDisciplineType
	{
		public DtoDisciplineType()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar" };
		}
		public DtoDisciplineType(int? id, string? disciplinetypename)
		{
			Id = id;
			DisciplineTypeName = disciplinetypename;
			TypeList = new(){ "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? DisciplineTypeName { get; set; }
		public List<string> TypeList { get; set; }
	}
}
