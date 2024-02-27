using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoDisciplineGroup
	{
		public DtoDisciplineGroup()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar" };
		}
		public DtoDisciplineGroup(int? id, string? disciplinegroupname)
		{
			Id = id;
			DisciplineGroupName = disciplinegroupname;
			TypeList = new(){ "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? DisciplineGroupName { get; set; }
		public List<string> TypeList { get; set; }
	}
}
