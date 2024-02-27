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
	public class LevelController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public LevelController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoLevel> GetLevel(DtoLevel Level, bool ExactFind = false)
		{
			DtoResult<DtoLevel> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Level.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Level) != null)
				{
					int index = Array.IndexOf(Level.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Level.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Level)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Level)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Level)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Level)}%'",
						};
					else
						condStr += Level.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Level)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Level)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Level)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Level)}",
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
					string sql = "SELECT * FROM Level" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoLevel> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoLevel
					{
						Id = x.Field<int?>("Id"),
						LevelName = x.Field<string?>("LevelName"),
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
		public ActionResult<DtoResult<DtoLevel>> Get()
		{
			DtoLevel dto = new();
			DtoResult<DtoLevel> result = GetLevel(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoLevel>> GetOne(DtoLevel Level)
		{
			DtoResult<DtoLevel> result = GetLevel(Level, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoLevel>> Find(DtoLevel Level)
		{
			DtoResult<DtoLevel> result = GetLevel(Level);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoLevel>> Add(DtoLevel Level)
		{
			DtoResult<DtoLevel>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("LevelAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@LevelName", SqlDbType.NVarChar).Value = Level.LevelName;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoLevel> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoLevel
						{
							Id = x.Field<int?>("Id"),
							LevelName = x.Field<string?>("LevelName"),
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
		public ActionResult<DtoResult<DtoLevel>> Update(DtoLevel Level)
		{
			DtoResult<DtoLevel>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("LevelUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Level.Id;
					cmd.Parameters.AddWithValue("@LevelName", SqlDbType.NVarChar).Value = Level.LevelName;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Level);
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
		public ActionResult<DtoResult<DtoLevel>> Delete(DtoLevel Level)
		{
			DtoResult<DtoLevel>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("LevelDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Level.Id;
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