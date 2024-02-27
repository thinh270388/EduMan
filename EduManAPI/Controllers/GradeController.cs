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
	public class GradeController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public GradeController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoGrade> GetGrade(DtoGrade Grade, bool ExactFind = false)
		{
			DtoResult<DtoGrade> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Grade.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Grade) != null)
				{
					int index = Array.IndexOf(Grade.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Grade.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Grade)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Grade)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Grade)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Grade)}%'",
						};
					else
						condStr += Grade.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Grade)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Grade)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Grade)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Grade)}",
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
					string sql = "SELECT * FROM Grade" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoGrade> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoGrade
					{
						Id = x.Field<int?>("Id"),
						GradeName = x.Field<string?>("GradeName"),
						LevelId = x.Field<int?>("LevelId"),
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
		public ActionResult<DtoResult<DtoGrade>> Get()
		{
			DtoGrade dto = new();
			DtoResult<DtoGrade> result = GetGrade(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoGrade>> GetOne(DtoGrade Grade)
		{
			DtoResult<DtoGrade> result = GetGrade(Grade, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoGrade>> Find(DtoGrade Grade)
		{
			DtoResult<DtoGrade> result = GetGrade(Grade);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoGrade>> Add(DtoGrade Grade)
		{
			DtoResult<DtoGrade>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("GradeAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@GradeName", SqlDbType.NVarChar).Value = Grade.GradeName;
					cmd.Parameters.AddWithValue("@LevelId", SqlDbType.Int).Value = Grade.LevelId;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoGrade> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoGrade
						{
							Id = x.Field<int?>("Id"),
							GradeName = x.Field<string?>("GradeName"),
							LevelId = x.Field<int?>("LevelId"),
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
		public ActionResult<DtoResult<DtoGrade>> Update(DtoGrade Grade)
		{
			DtoResult<DtoGrade>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("GradeUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Grade.Id;
					cmd.Parameters.AddWithValue("@GradeName", SqlDbType.NVarChar).Value = Grade.GradeName;
					cmd.Parameters.AddWithValue("@LevelId", SqlDbType.Int).Value = Grade.LevelId;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Grade);
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
		public ActionResult<DtoResult<DtoGrade>> Delete(DtoGrade Grade)
		{
			DtoResult<DtoGrade>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("GradeDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Grade.Id;
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