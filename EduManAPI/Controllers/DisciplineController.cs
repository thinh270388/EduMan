using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using EduManModel.Dtos;
using TextProcessing;
using System.Data;
using System.Reflection;

namespace EduManAPI.Controllers
{
	[Route("EduMan/[controller]")]
	[ApiController]
	public class DisciplineController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public DisciplineController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoDiscipline> GetDiscipline(DtoDiscipline Discipline, bool ExactFind = false)
		{
			DtoResult<DtoDiscipline> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Discipline.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Discipline) != null)
				{
					int index = Array.IndexOf(Discipline.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Discipline.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Discipline)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Discipline)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Discipline)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Discipline)}%'",
						};
					else
						condStr += Discipline.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Discipline)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Discipline)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Discipline)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Discipline)}",
						};
					}
			}
			if (condStr.Length > 0)
				condStr = string.Concat(" WHERE ", condStr.AsSpan(5, condStr.Length - 5));
			try
			{
				using (conn)
				{
					conn.Open();
					string sql = "SELECT * FROM Discipline" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoDiscipline> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDiscipline
					{
						Id = x.Field<int?>("Id"),
						DisciplineName = x.Field<string?>("DisciplineName"),
						DisciplineGroupId = x.Field<int?>("DisciplineGroupId"),
						ApplyFor = x.Field<int?>("ApplyFor"),
						PlusPoint = x.Field<int?>("PlusPoint"),
						MinusPoint = x.Field<int?>("MinusPoint"),
						Display = x.Field<bool?>("Display"),
						DisciplineTypeId = x.Field<int?>("DisciplineTypeId"),
						SequenceNumber = x.Field<int?>("SequenceNumber"),
						Note = x.Field<string?>("Note"),
					}).ToList();
					result.Message = "OK";
					if(!ExactFind)
						result.Results = rs;
					else
						result.Result = rs.FirstOrDefault()!;
				}
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}
			return result;
		}
		[HttpGet("GetAll")]
		public ActionResult<DtoResult<DtoDiscipline>> Get()
		{
			DtoDiscipline dto = new();
			DtoResult<DtoDiscipline> result = GetDiscipline(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoDiscipline>> GetOne(DtoDiscipline Discipline)
		{
			DtoResult<DtoDiscipline> result = GetDiscipline(Discipline, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoDiscipline>> Find(DtoDiscipline Discipline)
		{
			DtoResult<DtoDiscipline> result = GetDiscipline(Discipline);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoDiscipline>> Add(DtoDiscipline Discipline)
		{
			DtoResult<DtoDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@DisciplineName", SqlDbType.NVarChar).Value = Discipline.DisciplineName;
					cmd.Parameters.AddWithValue("@DisciplineGroupId", SqlDbType.Int).Value = Discipline.DisciplineGroupId;
					cmd.Parameters.AddWithValue("@ApplyFor", SqlDbType.Int).Value = Discipline.ApplyFor;
					cmd.Parameters.AddWithValue("@PlusPoint", SqlDbType.Int).Value = Discipline.PlusPoint;
					cmd.Parameters.AddWithValue("@MinusPoint", SqlDbType.Int).Value = Discipline.MinusPoint;
					cmd.Parameters.AddWithValue("@Display", SqlDbType.Bit).Value = Discipline.Display;
					cmd.Parameters.AddWithValue("@DisciplineTypeId", SqlDbType.Int).Value = Discipline.DisciplineTypeId;
					cmd.Parameters.AddWithValue("@SequenceNumber", SqlDbType.Int).Value = Discipline.SequenceNumber == null ? DBNull.Value : Discipline.SequenceNumber;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = Discipline.Note == null ? DBNull.Value : Discipline.Note;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoDiscipline> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDiscipline
						{
							Id = x.Field<int?>("Id"),
							DisciplineName = x.Field<string?>("DisciplineName"),
							DisciplineGroupId = x.Field<int?>("DisciplineGroupId"),
							ApplyFor = x.Field<int?>("ApplyFor"),
							PlusPoint = x.Field<int?>("PlusPoint"),
							MinusPoint = x.Field<int?>("MinusPoint"),
							Display = x.Field<bool?>("Display"),
							DisciplineTypeId = x.Field<int?>("DisciplineTypeId"),
							SequenceNumber = x.Field<int?>("SequenceNumber"),
							Note = x.Field<string?>("Note"),
						}).ToList();
						result.Message = "OK";
						result.Result = rs[0];
						return Ok(result);
					}
					else
						return BadRequest(result);
				}
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
				return Conflict(result);
			}
		}
		[HttpPost("Update")]
		public ActionResult<DtoResult<DtoDiscipline>> Update(DtoDiscipline Discipline)
		{
			DtoResult<DtoDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Discipline.Id;
					cmd.Parameters.AddWithValue("@DisciplineName", SqlDbType.NVarChar).Value = Discipline.DisciplineName;
					cmd.Parameters.AddWithValue("@DisciplineGroupId", SqlDbType.Int).Value = Discipline.DisciplineGroupId;
					cmd.Parameters.AddWithValue("@ApplyFor", SqlDbType.Int).Value = Discipline.ApplyFor;
					cmd.Parameters.AddWithValue("@PlusPoint", SqlDbType.Int).Value = Discipline.PlusPoint;
					cmd.Parameters.AddWithValue("@MinusPoint", SqlDbType.Int).Value = Discipline.MinusPoint;
					cmd.Parameters.AddWithValue("@Display", SqlDbType.Bit).Value = Discipline.Display;
					cmd.Parameters.AddWithValue("@DisciplineTypeId", SqlDbType.Int).Value = Discipline.DisciplineTypeId;
					cmd.Parameters.AddWithValue("@SequenceNumber", SqlDbType.Int).Value = Discipline.SequenceNumber == null ? DBNull.Value : Discipline.SequenceNumber;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = Discipline.Note == null ? DBNull.Value : Discipline.Note;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Discipline);
					}
					else
						return BadRequest(result);
				}
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
				return Conflict(result);
			}
		}
		[HttpPost("Delete")]
		public ActionResult<DtoResult<DtoDiscipline>> Delete(DtoDiscipline Discipline)
		{
			DtoResult<DtoDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Discipline.Id;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						result.Message = "OK";
					}
				}
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
				return Conflict(result);
			}
			return Ok(result);
		}
	}
}