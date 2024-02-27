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
	public class StartWeekController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public StartWeekController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoStartWeek> GetStartWeek(DtoStartWeek StartWeek, bool ExactFind = false)
		{
			DtoResult<DtoStartWeek> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in StartWeek.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(StartWeek) != null)
				{
					int index = Array.IndexOf(StartWeek.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += StartWeek.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(StartWeek)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(StartWeek)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(StartWeek)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(StartWeek)}%'",
						};
					else
						condStr += StartWeek.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(StartWeek)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(StartWeek)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(StartWeek)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(StartWeek)}",
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
					string sql = "SELECT * FROM StartWeek" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoStartWeek> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStartWeek
					{
						Id = x.Field<int?>("Id"),
						OnYear = x.Field<string?>("OnYear"),
						StartDate = x.Field<DateTime?>("StartDate"),
						Used = x.Field<bool?>("Used"),
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
		public ActionResult<DtoResult<DtoStartWeek>> Get()
		{
			DtoStartWeek dto = new();
			DtoResult<DtoStartWeek> result = GetStartWeek(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoStartWeek>> GetOne(DtoStartWeek StartWeek)
		{
			DtoResult<DtoStartWeek> result = GetStartWeek(StartWeek, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoStartWeek>> Find(DtoStartWeek StartWeek)
		{
			DtoResult<DtoStartWeek> result = GetStartWeek(StartWeek);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoStartWeek>> Add(DtoStartWeek StartWeek)
		{
			DtoResult<DtoStartWeek>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StartWeekAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@OnYear", SqlDbType.VarChar).Value = StartWeek.OnYear;
					cmd.Parameters.AddWithValue("@StartDate", SqlDbType.Date).Value = StartWeek.StartDate;
					cmd.Parameters.AddWithValue("@Used", SqlDbType.Bit).Value = StartWeek.Used;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoStartWeek> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStartWeek
						{
							Id = x.Field<int?>("Id"),
							OnYear = x.Field<string?>("OnYear"),
							StartDate = x.Field<DateTime?>("StartDate"),
							Used = x.Field<bool?>("Used"),
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
		public ActionResult<DtoResult<DtoStartWeek>> Update(DtoStartWeek StartWeek)
		{
			DtoResult<DtoStartWeek>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StartWeekUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = StartWeek.Id;
					cmd.Parameters.AddWithValue("@OnYear", SqlDbType.VarChar).Value = StartWeek.OnYear;
					cmd.Parameters.AddWithValue("@StartDate", SqlDbType.Date).Value = StartWeek.StartDate;
					cmd.Parameters.AddWithValue("@Used", SqlDbType.Bit).Value = StartWeek.Used;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(StartWeek);
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
		public ActionResult<DtoResult<DtoStartWeek>> Delete(DtoStartWeek StartWeek)
		{
			DtoResult<DtoStartWeek>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StartWeekDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = StartWeek.Id;
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