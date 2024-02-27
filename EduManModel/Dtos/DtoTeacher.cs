using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoTeacher
	{
		public DtoTeacher()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "varchar", "nvarchar", "nvarchar", "nvarchar" };
		}
		public DtoTeacher(int? id, string? code, string? fullname, string? phone, string? email)
		{
			Id = id;
			Code = code;
			FullName = fullname;
			Phone = phone;
			Email = email;
			TypeList = new(){ "int", "varchar", "nvarchar", "nvarchar", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? Code { get; set; }
		public string? FullName { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public List<string> TypeList { get; set; }
	}
}
