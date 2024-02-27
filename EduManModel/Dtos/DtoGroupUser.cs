using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoGroupUser
	{
		public DtoGroupUser()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar" };
		}
		public DtoGroupUser(int? id, string? groupusername)
		{
			Id = id;
			GroupUserName = groupusername;
			TypeList = new(){ "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? GroupUserName { get; set; }
		public List<string> TypeList { get; set; }
	}
}
