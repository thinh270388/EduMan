using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoDroppedOut
	{
		public DtoDroppedOut()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "int", "nvarchar", "date", "nvarchar", "nvarchar", "date", "nvarchar" };
		}
		public DtoDroppedOut(int? id, int? studentid, string? semaster, DateTime? ondate, string? reason, string? decisionnumber, DateTime? decisiondate, string? note)
		{
			Id = id;
			StudentId = studentid;
			Semaster = semaster;
			OnDate = ondate;
			Reason = reason;
			DecisionNumber = decisionnumber;
			DecisionDate = decisiondate;
			Note = note;
			TypeList = new(){ "int", "int", "nvarchar", "date", "nvarchar", "nvarchar", "date", "nvarchar" };
		}
		public int? Id { get; set; }
		public int? StudentId { get; set; }
		public string? Semaster { get; set; }
		public DateTime? OnDate { get; set; }
		public string? Reason { get; set; }
		public string? DecisionNumber { get; set; }
		public DateTime? DecisionDate { get; set; }
		public string? Note { get; set; }
		public List<string> TypeList { get; set; }
	}
}
