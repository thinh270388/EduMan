using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EduManModel.Dtos
{
	public class DtoDiscipline
	{
		public DtoDiscipline()
		{
			List<PropertyInfo> properties = GetType().GetProperties().ToList();
			foreach (PropertyInfo property in properties)
			{
				property.SetValue(this, null);
			}
			TypeList = new(){ "int", "nvarchar", "int", "int", "int", "int", "bit", "int", "int", "nvarchar" };
		}
		public DtoDiscipline(int? id, string? disciplinename, int? disciplinegroupid, int? applyfor, int? pluspoint, int? minuspoint, bool? display, int? disciplinetypeid, int? sequencenumber, string? note)
		{
			Id = id;
			DisciplineName = disciplinename;
			DisciplineGroupId = disciplinegroupid;
			ApplyFor = applyfor;
			PlusPoint = pluspoint;
			MinusPoint = minuspoint;
			Display = display;
			DisciplineTypeId = disciplinetypeid;
			SequenceNumber = sequencenumber;
			Note = note;
			TypeList = new(){ "int", "nvarchar", "int", "int", "int", "int", "bit", "int", "int", "nvarchar" };
		}
		public int? Id { get; set; }
		public string? DisciplineName { get; set; }
		public int? DisciplineGroupId { get; set; }
		public int? ApplyFor { get; set; }
		public int? PlusPoint { get; set; }
		public int? MinusPoint { get; set; }
		public bool? Display { get; set; }
		public int? DisciplineTypeId { get; set; }
		public int? SequenceNumber { get; set; }
		public string? Note { get; set; }
		public List<string> TypeList { get; set; }
	}
}
