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
	public class WeeklyController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public WeeklyController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoWeekly> GetWeekly(DtoWeekly Weekly, bool ExactFind = false)
		{
			DtoResult<DtoWeekly> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Weekly.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Weekly) != null)
				{
					int index = Array.IndexOf(Weekly.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Weekly.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Weekly)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Weekly)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Weekly)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Weekly)}%'",
						};
					else
						condStr += Weekly.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Weekly)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Weekly)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Weekly)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Weekly)}",
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
					string sql = "SELECT * FROM Weekly" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoWeekly> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoWeekly
					{
						Id = x.Field<int?>("Id"),
						StartWeekId = x.Field<int?>("StartWeekId"),
						WeeklyName = x.Field<string?>("WeeklyName"),
						FromDate = x.Field<DateTime?>("FromDate"),
						ToDate = x.Field<DateTime?>("ToDate"),
						NumberOfLession = x.Field<int?>("NumberOfLession"),
						InitialPoint = x.Field<int?>("InitialPoint"),
						Coefficient = x.Field<int?>("Coefficient"),
						OnDutyClass = x.Field<string?>("OnDutyClass"),
						Sumarizing = x.Field<string?>("Sumarizing"),
						Planning = x.Field<string?>("Planning"),
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
		public ActionResult<DtoResult<DtoWeekly>> Get()
		{
			DtoWeekly dto = new();
			DtoResult<DtoWeekly> result = GetWeekly(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoWeekly>> GetOne(DtoWeekly Weekly)
		{
			DtoResult<DtoWeekly> result = GetWeekly(Weekly, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoWeekly>> Find(DtoWeekly Weekly)
		{
			DtoResult<DtoWeekly> result = GetWeekly(Weekly);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoWeekly>> Add(DtoWeekly Weekly)
		{
			DtoResult<DtoWeekly>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("WeeklyAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@StartWeekId", SqlDbType.Int).Value = Weekly.StartWeekId;
					cmd.Parameters.AddWithValue("@WeeklyName", SqlDbType.NVarChar).Value = Weekly.WeeklyName;
					cmd.Parameters.AddWithValue("@FromDate", SqlDbType.Date).Value = Weekly.FromDate;
					cmd.Parameters.AddWithValue("@ToDate", SqlDbType.Date).Value = Weekly.ToDate;
					cmd.Parameters.AddWithValue("@NumberOfLession", SqlDbType.Int).Value = Weekly.NumberOfLession;
					cmd.Parameters.AddWithValue("@InitialPoint", SqlDbType.Int).Value = Weekly.InitialPoint;
					cmd.Parameters.AddWithValue("@Coefficient", SqlDbType.Int).Value = Weekly.Coefficient;
					cmd.Parameters.AddWithValue("@OnDutyClass", SqlDbType.NVarChar).Value = Weekly.OnDutyClass == null ? DBNull.Value : Weekly.OnDutyClass;
					cmd.Parameters.AddWithValue("@Sumarizing", SqlDbType.NVarChar).Value = Weekly.Sumarizing == null ? DBNull.Value : Weekly.Sumarizing;
					cmd.Parameters.AddWithValue("@Planning", SqlDbType.NVarChar).Value = Weekly.Planning == null ? DBNull.Value : Weekly.Planning;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoWeekly> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoWeekly
						{
							Id = x.Field<int?>("Id"),
							StartWeekId = x.Field<int?>("StartWeekId"),
							WeeklyName = x.Field<string?>("WeeklyName"),
							FromDate = x.Field<DateTime?>("FromDate"),
							ToDate = x.Field<DateTime?>("ToDate"),
							NumberOfLession = x.Field<int?>("NumberOfLession"),
							InitialPoint = x.Field<int?>("InitialPoint"),
							Coefficient = x.Field<int?>("Coefficient"),
							OnDutyClass = x.Field<string?>("OnDutyClass"),
							Sumarizing = x.Field<string?>("Sumarizing"),
							Planning = x.Field<string?>("Planning"),
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
		public ActionResult<DtoResult<DtoWeekly>> Update(DtoWeekly Weekly)
		{
			DtoResult<DtoWeekly>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("WeeklyUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Weekly.Id;
					cmd.Parameters.AddWithValue("@StartWeekId", SqlDbType.Int).Value = Weekly.StartWeekId;
					cmd.Parameters.AddWithValue("@WeeklyName", SqlDbType.NVarChar).Value = Weekly.WeeklyName;
					cmd.Parameters.AddWithValue("@FromDate", SqlDbType.Date).Value = Weekly.FromDate;
					cmd.Parameters.AddWithValue("@ToDate", SqlDbType.Date).Value = Weekly.ToDate;
					cmd.Parameters.AddWithValue("@NumberOfLession", SqlDbType.Int).Value = Weekly.NumberOfLession;
					cmd.Parameters.AddWithValue("@InitialPoint", SqlDbType.Int).Value = Weekly.InitialPoint;
					cmd.Parameters.AddWithValue("@Coefficient", SqlDbType.Int).Value = Weekly.Coefficient;
					cmd.Parameters.AddWithValue("@OnDutyClass", SqlDbType.NVarChar).Value = Weekly.OnDutyClass == null ? DBNull.Value : Weekly.OnDutyClass;
					cmd.Parameters.AddWithValue("@Sumarizing", SqlDbType.NVarChar).Value = Weekly.Sumarizing == null ? DBNull.Value : Weekly.Sumarizing;
					cmd.Parameters.AddWithValue("@Planning", SqlDbType.NVarChar).Value = Weekly.Planning == null ? DBNull.Value : Weekly.Planning;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Weekly);
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
		public ActionResult<DtoResult<DtoWeekly>> Delete(DtoWeekly Weekly)
		{
			DtoResult<DtoWeekly>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("WeeklyDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Weekly.Id;
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