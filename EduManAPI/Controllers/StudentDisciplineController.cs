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
	public class StudentDisciplineController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public StudentDisciplineController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoStudentDiscipline> GetStudentDiscipline(DtoStudentDiscipline StudentDiscipline, bool ExactFind = false)
		{
			DtoResult<DtoStudentDiscipline> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in StudentDiscipline.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(StudentDiscipline) != null)
				{
					int index = Array.IndexOf(StudentDiscipline.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += StudentDiscipline.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(StudentDiscipline)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(StudentDiscipline)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(StudentDiscipline)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(StudentDiscipline)}%'",
						};
					else
						condStr += StudentDiscipline.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(StudentDiscipline)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(StudentDiscipline)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(StudentDiscipline)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(StudentDiscipline)}",
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
					string sql = "SELECT * FROM StudentDiscipline" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoStudentDiscipline> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStudentDiscipline
					{
						Id = x.Field<int?>("Id"),
						StudentId = x.Field<int?>("StudentId"),
						DisciplineId = x.Field<int?>("DisciplineId"),
						WeeklyId = x.Field<int?>("WeeklyId"),
						OnDate = x.Field<DateTime?>("OnDate"),
						Times = x.Field<int?>("Times"),
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
		public ActionResult<DtoResult<DtoStudentDiscipline>> Get()
		{
			DtoStudentDiscipline dto = new();
			DtoResult<DtoStudentDiscipline> result = GetStudentDiscipline(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoStudentDiscipline>> GetOne(DtoStudentDiscipline StudentDiscipline)
		{
			DtoResult<DtoStudentDiscipline> result = GetStudentDiscipline(StudentDiscipline, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoStudentDiscipline>> Find(DtoStudentDiscipline StudentDiscipline)
		{
			DtoResult<DtoStudentDiscipline> result = GetStudentDiscipline(StudentDiscipline);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoStudentDiscipline>> Add(DtoStudentDiscipline StudentDiscipline)
		{
			DtoResult<DtoStudentDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDisciplineAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentDiscipline.StudentId;
					cmd.Parameters.AddWithValue("@DisciplineId", SqlDbType.Int).Value = StudentDiscipline.DisciplineId;
					cmd.Parameters.AddWithValue("@WeeklyId", SqlDbType.Int).Value = StudentDiscipline.WeeklyId;
					cmd.Parameters.AddWithValue("@OnDate", SqlDbType.Date).Value = StudentDiscipline.OnDate;
					cmd.Parameters.AddWithValue("@Times", SqlDbType.Int).Value = StudentDiscipline.Times;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoStudentDiscipline> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStudentDiscipline
						{
							Id = x.Field<int?>("Id"),
							StudentId = x.Field<int?>("StudentId"),
							DisciplineId = x.Field<int?>("DisciplineId"),
							WeeklyId = x.Field<int?>("WeeklyId"),
							OnDate = x.Field<DateTime?>("OnDate"),
							Times = x.Field<int?>("Times"),
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
		public ActionResult<DtoResult<DtoStudentDiscipline>> Update(DtoStudentDiscipline StudentDiscipline)
		{
			DtoResult<DtoStudentDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDisciplineUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = StudentDiscipline.Id;
					cmd.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentDiscipline.StudentId;
					cmd.Parameters.AddWithValue("@DisciplineId", SqlDbType.Int).Value = StudentDiscipline.DisciplineId;
					cmd.Parameters.AddWithValue("@WeeklyId", SqlDbType.Int).Value = StudentDiscipline.WeeklyId;
					cmd.Parameters.AddWithValue("@OnDate", SqlDbType.Date).Value = StudentDiscipline.OnDate;
					cmd.Parameters.AddWithValue("@Times", SqlDbType.Int).Value = StudentDiscipline.Times;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(StudentDiscipline);
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
		public ActionResult<DtoResult<DtoStudentDiscipline>> Delete(DtoStudentDiscipline StudentDiscipline)
		{
			DtoResult<DtoStudentDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDisciplineDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = StudentDiscipline.Id;
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