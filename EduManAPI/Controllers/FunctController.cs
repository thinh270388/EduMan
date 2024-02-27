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
	public class FunctController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public FunctController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoFunct> GetFunct(DtoFunct Funct, bool ExactFind = false)
		{
			DtoResult<DtoFunct> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Funct.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Funct) != null)
				{
					int index = Array.IndexOf(Funct.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Funct.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Funct)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Funct)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Funct)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Funct)}%'",
						};
					else
						condStr += Funct.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Funct)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Funct)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Funct)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Funct)}",
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
					string sql = "SELECT * FROM Funct" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoFunct> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoFunct
					{
						Id = x.Field<int?>("Id"),
						FunctName = x.Field<string?>("FunctName"),
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
		public ActionResult<DtoResult<DtoFunct>> Get()
		{
			DtoFunct dto = new();
			DtoResult<DtoFunct> result = GetFunct(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoFunct>> GetOne(DtoFunct Funct)
		{
			DtoResult<DtoFunct> result = GetFunct(Funct, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoFunct>> Find(DtoFunct Funct)
		{
			DtoResult<DtoFunct> result = GetFunct(Funct);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoFunct>> Add(DtoFunct Funct)
		{
			DtoResult<DtoFunct>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("FunctAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@FunctName", SqlDbType.NVarChar).Value = Funct.FunctName;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoFunct> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoFunct
						{
							Id = x.Field<int?>("Id"),
							FunctName = x.Field<string?>("FunctName"),
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
		public ActionResult<DtoResult<DtoFunct>> Update(DtoFunct Funct)
		{
			DtoResult<DtoFunct>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("FunctUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Funct.Id;
					cmd.Parameters.AddWithValue("@FunctName", SqlDbType.NVarChar).Value = Funct.FunctName;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Funct);
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
		public ActionResult<DtoResult<DtoFunct>> Delete(DtoFunct Funct)
		{
			DtoResult<DtoFunct>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("FunctDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Funct.Id;
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