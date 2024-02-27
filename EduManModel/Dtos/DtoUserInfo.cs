using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoUserInfo
	{
		public DtoUserInfo()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "varchar", "nvarchar", "nvarchar", "int", "bit" };
		}
		public DtoUserInfo(int? id, string? username, string? userpassword, string? fullname, int? groupuserid, bool? active)
		{
			Id = id;
			UserName = username;
			UserPassword = userpassword;
			FullName = fullname;
			GroupUserId = groupuserid;
			Active = active;
			TypeList = new(){ "int", "varchar", "nvarchar", "nvarchar", "int", "bit" };
		}
		public int? Id { get; set; }
		public string? UserName { get; set; }
		public string? UserPassword { get; set; }
		public string? FullName { get; set; }
		public int? GroupUserId { get; set; }
		public bool? Active { get; set; }
		public List<string> TypeList { get; set; }
	}
}
