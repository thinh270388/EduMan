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
	public class TeacherController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public TeacherController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoTeacher> GetTeacher(DtoTeacher Teacher, bool ExactFind = false)
		{
			DtoResult<DtoTeacher> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Teacher.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Teacher) != null)
				{
					int index = Array.IndexOf(Teacher.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Teacher.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Teacher)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Teacher)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Teacher)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Teacher)}%'",
						};
					else
						condStr += Teacher.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Teacher)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Teacher)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Teacher)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Teacher)}",
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
					string sql = "SELECT * FROM Teacher" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoTeacher> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoTeacher
					{
						Id = x.Field<int?>("Id"),
						Code = x.Field<string?>("Code"),
						FullName = x.Field<string?>("FullName"),
						Phone = x.Field<string?>("Phone"),
						Email = x.Field<string?>("Email"),
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
		public ActionResult<DtoResult<DtoTeacher>> Get()
		{
			DtoTeacher dto = new();
			DtoResult<DtoTeacher> result = GetTeacher(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoTeacher>> GetOne(DtoTeacher Teacher)
		{
			DtoResult<DtoTeacher> result = GetTeacher(Teacher, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoTeacher>> Find(DtoTeacher Teacher)
		{
			DtoResult<DtoTeacher> result = GetTeacher(Teacher);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoTeacher>> Add(DtoTeacher Teacher)
		{
			DtoResult<DtoTeacher>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("TeacherAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Code", SqlDbType.VarChar).Value = Teacher.Code;
					cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = Teacher.FullName;
					cmd.Parameters.AddWithValue("@Phone", SqlDbType.NVarChar).Value = Teacher.Phone == null ? DBNull.Value : Teacher.Phone;
					cmd.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = Teacher.Email == null ? DBNull.Value : Teacher.Email;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoTeacher> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoTeacher
						{
							Id = x.Field<int?>("Id"),
							Code = x.Field<string?>("Code"),
							FullName = x.Field<string?>("FullName"),
							Phone = x.Field<string?>("Phone"),
							Email = x.Field<string?>("Email"),
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
		public ActionResult<DtoResult<DtoTeacher>> Update(DtoTeacher Teacher)
		{
			DtoResult<DtoTeacher>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("TeacherUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Teacher.Id;
					cmd.Parameters.AddWithValue("@Code", SqlDbType.VarChar).Value = Teacher.Code;
					cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = Teacher.FullName;
					cmd.Parameters.AddWithValue("@Phone", SqlDbType.NVarChar).Value = Teacher.Phone == null ? DBNull.Value : Teacher.Phone;
					cmd.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = Teacher.Email == null ? DBNull.Value : Teacher.Email;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Teacher);
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
		public ActionResult<DtoResult<DtoTeacher>> Delete(DtoTeacher Teacher)
		{
			DtoResult<DtoTeacher>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("TeacherDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Teacher.Id;
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