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
	public class GroupUserController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public GroupUserController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoGroupUser> GetGroupUser(DtoGroupUser GroupUser, bool ExactFind = false)
		{
			DtoResult<DtoGroupUser> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in GroupUser.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(GroupUser) != null)
				{
					int index = Array.IndexOf(GroupUser.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += GroupUser.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(GroupUser)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(GroupUser)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(GroupUser)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(GroupUser)}%'",
						};
					else
						condStr += GroupUser.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(GroupUser)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(GroupUser)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(GroupUser)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(GroupUser)}",
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
					string sql = "SELECT * FROM GroupUser" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoGroupUser> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoGroupUser
					{
						Id = x.Field<int?>("Id"),
						GroupUserName = x.Field<string?>("GroupUserName"),
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
		public ActionResult<DtoResult<DtoGroupUser>> Get()
		{
			DtoGroupUser dto = new();
			DtoResult<DtoGroupUser> result = GetGroupUser(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoGroupUser>> GetOne(DtoGroupUser GroupUser)
		{
			DtoResult<DtoGroupUser> result = GetGroupUser(GroupUser, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoGroupUser>> Find(DtoGroupUser GroupUser)
		{
			DtoResult<DtoGroupUser> result = GetGroupUser(GroupUser);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoGroupUser>> Add(DtoGroupUser GroupUser)
		{
			DtoResult<DtoGroupUser>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("GroupUserAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@GroupUserName", SqlDbType.NVarChar).Value = GroupUser.GroupUserName;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoGroupUser> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoGroupUser
						{
							Id = x.Field<int?>("Id"),
							GroupUserName = x.Field<string?>("GroupUserName"),
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
		public ActionResult<DtoResult<DtoGroupUser>> Update(DtoGroupUser GroupUser)
		{
			DtoResult<DtoGroupUser>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("GroupUserUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = GroupUser.Id;
					cmd.Parameters.AddWithValue("@GroupUserName", SqlDbType.NVarChar).Value = GroupUser.GroupUserName;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(GroupUser);
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
		public ActionResult<DtoResult<DtoGroupUser>> Delete(DtoGroupUser GroupUser)
		{
			DtoResult<DtoGroupUser>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("GroupUserDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = GroupUser.Id;
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