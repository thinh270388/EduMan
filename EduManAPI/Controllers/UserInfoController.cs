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
	public class UserInfoController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public UserInfoController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoUserInfo> GetUserInfo(DtoUserInfo UserInfo, bool ExactFind = false)
		{
			DtoResult<DtoUserInfo> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in UserInfo.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(UserInfo) != null)
				{
					int index = Array.IndexOf(UserInfo.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += UserInfo.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(UserInfo)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(UserInfo)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(UserInfo)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(UserInfo)}%'",
						};
					else
						condStr += UserInfo.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(UserInfo)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(UserInfo)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(UserInfo)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(UserInfo)}",
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
					string sql = "SELECT * FROM UserInfo" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoUserInfo> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoUserInfo
					{
						Id = x.Field<int?>("Id"),
						UserName = x.Field<string?>("UserName"),
						UserPassword = x.Field<string?>("UserPassword"),
						FullName = x.Field<string?>("FullName"),
						GroupUserId = x.Field<int?>("GroupUserId"),
						Active = x.Field<bool?>("Active"),
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
		public ActionResult<DtoResult<DtoUserInfo>> Get()
		{
			DtoUserInfo dto = new();
			DtoResult<DtoUserInfo> result = GetUserInfo(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoUserInfo>> GetOne(DtoUserInfo UserInfo)
		{
			DtoResult<DtoUserInfo> result = GetUserInfo(UserInfo, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoUserInfo>> Find(DtoUserInfo UserInfo)
		{
			DtoResult<DtoUserInfo> result = GetUserInfo(UserInfo);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoUserInfo>> Add(DtoUserInfo UserInfo)
		{
			DtoResult<DtoUserInfo>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("UserInfoAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@UserName", SqlDbType.VarChar).Value = UserInfo.UserName;
					cmd.Parameters.AddWithValue("@UserPassword", SqlDbType.NVarChar).Value = UserInfo.UserPassword;
					cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = UserInfo.FullName == null ? DBNull.Value : UserInfo.FullName;
					cmd.Parameters.AddWithValue("@GroupUserId", SqlDbType.Int).Value = UserInfo.GroupUserId;
					cmd.Parameters.AddWithValue("@Active", SqlDbType.Bit).Value = UserInfo.Active;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoUserInfo> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoUserInfo
						{
							Id = x.Field<int?>("Id"),
							UserName = x.Field<string?>("UserName"),
							UserPassword = x.Field<string?>("UserPassword"),
							FullName = x.Field<string?>("FullName"),
							GroupUserId = x.Field<int?>("GroupUserId"),
							Active = x.Field<bool?>("Active"),
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
		public ActionResult<DtoResult<DtoUserInfo>> Update(DtoUserInfo UserInfo)
		{
			DtoResult<DtoUserInfo>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("UserInfoUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = UserInfo.Id;
					cmd.Parameters.AddWithValue("@UserName", SqlDbType.VarChar).Value = UserInfo.UserName;
					cmd.Parameters.AddWithValue("@UserPassword", SqlDbType.NVarChar).Value = UserInfo.UserPassword;
					cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = UserInfo.FullName == null ? DBNull.Value : UserInfo.FullName;
					cmd.Parameters.AddWithValue("@GroupUserId", SqlDbType.Int).Value = UserInfo.GroupUserId;
					cmd.Parameters.AddWithValue("@Active", SqlDbType.Bit).Value = UserInfo.Active;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(UserInfo);
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
		public ActionResult<DtoResult<DtoUserInfo>> Delete(DtoUserInfo UserInfo)
		{
			DtoResult<DtoUserInfo>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("UserInfoDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = UserInfo.Id;
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