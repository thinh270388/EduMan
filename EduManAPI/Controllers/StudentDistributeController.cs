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
	public class StudentDistributeController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public StudentDistributeController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoStudentDistribute> GetStudentDistribute(DtoStudentDistribute StudentDistribute, bool ExactFind = false)
		{
			DtoResult<DtoStudentDistribute> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in StudentDistribute.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(StudentDistribute) != null)
				{
					int index = Array.IndexOf(StudentDistribute.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += StudentDistribute.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(StudentDistribute)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(StudentDistribute)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(StudentDistribute)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(StudentDistribute)}%'",
						};
					else
						condStr += StudentDistribute.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(StudentDistribute)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(StudentDistribute)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(StudentDistribute)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(StudentDistribute)}",
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
					string sql = "SELECT * FROM StudentDistribute" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoStudentDistribute> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStudentDistribute
					{
						Id = x.Field<int?>("Id"),
						ClassDistributeId = x.Field<int?>("ClassDistributeId"),
						StudentId = x.Field<int?>("StudentId"),
						AssignDate = x.Field<DateTime?>("AssignDate"),
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
		public ActionResult<DtoResult<DtoStudentDistribute>> Get()
		{
			DtoStudentDistribute dto = new();
			DtoResult<DtoStudentDistribute> result = GetStudentDistribute(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoStudentDistribute>> GetOne(DtoStudentDistribute StudentDistribute)
		{
			DtoResult<DtoStudentDistribute> result = GetStudentDistribute(StudentDistribute, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoStudentDistribute>> Find(DtoStudentDistribute StudentDistribute)
		{
			DtoResult<DtoStudentDistribute> result = GetStudentDistribute(StudentDistribute);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoStudentDistribute>> Add(DtoStudentDistribute StudentDistribute)
		{
			DtoResult<DtoStudentDistribute>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDistributeAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@ClassDistributeId", SqlDbType.Int).Value = StudentDistribute.ClassDistributeId;
					cmd.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentDistribute.StudentId;
					cmd.Parameters.AddWithValue("@AssignDate", SqlDbType.Date).Value = StudentDistribute.AssignDate;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = StudentDistribute.Note == null ? DBNull.Value : StudentDistribute.Note;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoStudentDistribute> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStudentDistribute
						{
							Id = x.Field<int?>("Id"),
							ClassDistributeId = x.Field<int?>("ClassDistributeId"),
							StudentId = x.Field<int?>("StudentId"),
							AssignDate = x.Field<DateTime?>("AssignDate"),
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
		public ActionResult<DtoResult<DtoStudentDistribute>> Update(DtoStudentDistribute StudentDistribute)
		{
			DtoResult<DtoStudentDistribute>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDistributeUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = StudentDistribute.Id;
					cmd.Parameters.AddWithValue("@ClassDistributeId", SqlDbType.Int).Value = StudentDistribute.ClassDistributeId;
					cmd.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = StudentDistribute.StudentId;
					cmd.Parameters.AddWithValue("@AssignDate", SqlDbType.Date).Value = StudentDistribute.AssignDate;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = StudentDistribute.Note == null ? DBNull.Value : StudentDistribute.Note;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(StudentDistribute);
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
		public ActionResult<DtoResult<DtoStudentDistribute>> Delete(DtoStudentDistribute StudentDistribute)
		{
			DtoResult<DtoStudentDistribute>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDistributeDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = StudentDistribute.Id;
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