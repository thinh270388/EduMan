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
	public class DroppedOutController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public DroppedOutController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoDroppedOut> GetDroppedOut(DtoDroppedOut DroppedOut, bool ExactFind = false)
		{
			DtoResult<DtoDroppedOut> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in DroppedOut.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(DroppedOut) != null)
				{
					int index = Array.IndexOf(DroppedOut.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += DroppedOut.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(DroppedOut)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(DroppedOut)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(DroppedOut)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(DroppedOut)}%'",
						};
					else
						condStr += DroppedOut.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(DroppedOut)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(DroppedOut)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(DroppedOut)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(DroppedOut)}",
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
					string sql = "SELECT * FROM DroppedOut" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoDroppedOut> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDroppedOut
					{
						Id = x.Field<int?>("Id"),
						StudentId = x.Field<int?>("StudentId"),
						Semaster = x.Field<string?>("Semaster"),
						OnDate = x.Field<DateTime?>("OnDate"),
						Reason = x.Field<string?>("Reason"),
						DecisionNumber = x.Field<string?>("DecisionNumber"),
						DecisionDate = x.Field<DateTime?>("DecisionDate"),
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
		public ActionResult<DtoResult<DtoDroppedOut>> Get()
		{
			DtoDroppedOut dto = new();
			DtoResult<DtoDroppedOut> result = GetDroppedOut(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoDroppedOut>> GetOne(DtoDroppedOut DroppedOut)
		{
			DtoResult<DtoDroppedOut> result = GetDroppedOut(DroppedOut, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoDroppedOut>> Find(DtoDroppedOut DroppedOut)
		{
			DtoResult<DtoDroppedOut> result = GetDroppedOut(DroppedOut);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoDroppedOut>> Add(DtoDroppedOut DroppedOut)
		{
			DtoResult<DtoDroppedOut>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DroppedOutAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = DroppedOut.StudentId;
					cmd.Parameters.AddWithValue("@Semaster", SqlDbType.NVarChar).Value = DroppedOut.Semaster;
					cmd.Parameters.AddWithValue("@OnDate", SqlDbType.Date).Value = DroppedOut.OnDate;
					cmd.Parameters.AddWithValue("@Reason", SqlDbType.NVarChar).Value = DroppedOut.Reason == null ? DBNull.Value : DroppedOut.Reason;
					cmd.Parameters.AddWithValue("@DecisionNumber", SqlDbType.NVarChar).Value = DroppedOut.DecisionNumber == null ? DBNull.Value : DroppedOut.DecisionNumber;
					cmd.Parameters.AddWithValue("@DecisionDate", SqlDbType.Date).Value = DroppedOut.DecisionDate == null ? DBNull.Value : DroppedOut.DecisionDate;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = DroppedOut.Note == null ? DBNull.Value : DroppedOut.Note;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoDroppedOut> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDroppedOut
						{
							Id = x.Field<int?>("Id"),
							StudentId = x.Field<int?>("StudentId"),
							Semaster = x.Field<string?>("Semaster"),
							OnDate = x.Field<DateTime?>("OnDate"),
							Reason = x.Field<string?>("Reason"),
							DecisionNumber = x.Field<string?>("DecisionNumber"),
							DecisionDate = x.Field<DateTime?>("DecisionDate"),
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
		public ActionResult<DtoResult<DtoDroppedOut>> Update(DtoDroppedOut DroppedOut)
		{
			DtoResult<DtoDroppedOut>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DroppedOutUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = DroppedOut.Id;
					cmd.Parameters.AddWithValue("@StudentId", SqlDbType.Int).Value = DroppedOut.StudentId;
					cmd.Parameters.AddWithValue("@Semaster", SqlDbType.NVarChar).Value = DroppedOut.Semaster;
					cmd.Parameters.AddWithValue("@OnDate", SqlDbType.Date).Value = DroppedOut.OnDate;
					cmd.Parameters.AddWithValue("@Reason", SqlDbType.NVarChar).Value = DroppedOut.Reason == null ? DBNull.Value : DroppedOut.Reason;
					cmd.Parameters.AddWithValue("@DecisionNumber", SqlDbType.NVarChar).Value = DroppedOut.DecisionNumber == null ? DBNull.Value : DroppedOut.DecisionNumber;
					cmd.Parameters.AddWithValue("@DecisionDate", SqlDbType.Date).Value = DroppedOut.DecisionDate == null ? DBNull.Value : DroppedOut.DecisionDate;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = DroppedOut.Note == null ? DBNull.Value : DroppedOut.Note;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(DroppedOut);
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
		public ActionResult<DtoResult<DtoDroppedOut>> Delete(DtoDroppedOut DroppedOut)
		{
			DtoResult<DtoDroppedOut>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DroppedOutDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = DroppedOut.Id;
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