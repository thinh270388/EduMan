using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoRoleAssign
	{
		public DtoRoleAssign()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "int", "int" };
		}
		public DtoRoleAssign(int? id, int? groupuserid, int? functid)
		{
			Id = id;
			GroupUserId = groupuserid;
			FunctId = functid;
			TypeList = new(){ "int", "int", "int" };
		}
		public int? Id { get; set; }
		public int? GroupUserId { get; set; }
		public int? FunctId { get; set; }
		public List<string> TypeList { get; set; }
	}
}
