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
	public class ClassController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public ClassController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoClass> GetClass(DtoClass Class, bool ExactFind = false)
		{
			DtoResult<DtoClass> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Class.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Class) != null)
				{
					int index = Array.IndexOf(Class.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Class.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Class)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Class)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Class)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Class)}%'",
						};
					else
						condStr += Class.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Class)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Class)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Class)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Class)}",
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
					string sql = "SELECT * FROM Class" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoClass> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoClass
					{
						Id = x.Field<int?>("Id"),
						ClassName = x.Field<string?>("ClassName"),
						GradeId = x.Field<int?>("GradeId"),
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
		public ActionResult<DtoResult<DtoClass>> Get()
		{
			DtoClass dto = new();
			DtoResult<DtoClass> result = GetClass(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoClass>> GetOne(DtoClass Class)
		{
			DtoResult<DtoClass> result = GetClass(Class, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoClass>> Find(DtoClass Class)
		{
			DtoResult<DtoClass> result = GetClass(Class);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoClass>> Add(DtoClass Class)
		{
			DtoResult<DtoClass>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@ClassName", SqlDbType.NVarChar).Value = Class.ClassName;
					cmd.Parameters.AddWithValue("@GradeId", SqlDbType.Int).Value = Class.GradeId;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoClass> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoClass
						{
							Id = x.Field<int?>("Id"),
							ClassName = x.Field<string?>("ClassName"),
							GradeId = x.Field<int?>("GradeId"),
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
		public ActionResult<DtoResult<DtoClass>> Update(DtoClass Class)
		{
			DtoResult<DtoClass>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Class.Id;
					cmd.Parameters.AddWithValue("@ClassName", SqlDbType.NVarChar).Value = Class.ClassName;
					cmd.Parameters.AddWithValue("@GradeId", SqlDbType.Int).Value = Class.GradeId;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Class);
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
		public ActionResult<DtoResult<DtoClass>> Delete(DtoClass Class)
		{
			DtoResult<DtoClass>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Class.Id;
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