using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoStudent
	{
		public DtoStudent()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "varchar", "nvarchar", "date", "bit", "nvarchar", "nvarchar", "nvarchar", "nvarchar", "int", "nvarchar" };
		}
		public DtoStudent(int? id, string? code, string? fullname, DateTime? birthday, bool? gender, string? phone, string? email, string? addresscurrent, string? contactinfo, int? sequencenumber, string? note)
		{
			Id = id;
			Code = code;
			FullName = fullname;
			Birthday = birthday;
			Gender = gender;
			Phone = phone;
			Email = email;
			AddressCurrent = addresscurrent;
			ContactInfo = contactinfo;
			SequenceNumber = sequencenumber;
			Note = note;
			TypeList = new(){ "int", "varchar", "nvarchar", "date", "bit", "nvarchar", "nvarchar", "nvarchar", "nvarchar", "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? Code { get; set; }
		public string? FullName { get; set; }
		public DateTime? Birthday { get; set; }
		public bool? Gender { get; set; }
		public string? Phone { get; set; }
		public string? Email { get; set; }
		public string? AddressCurrent { get; set; }
		public string? ContactInfo { get; set; }
		public int? SequenceNumber { get; set; }
		public string? Note { get; set; }
		public List<string> TypeList { get; set; }
	}
}
