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
	public class RoleAssignController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public RoleAssignController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoRoleAssign> GetRoleAssign(DtoRoleAssign RoleAssign, bool ExactFind = false)
		{
			DtoResult<DtoRoleAssign> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in RoleAssign.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(RoleAssign) != null)
				{
					int index = Array.IndexOf(RoleAssign.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += RoleAssign.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(RoleAssign)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(RoleAssign)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(RoleAssign)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(RoleAssign)}%'",
						};
					else
						condStr += RoleAssign.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(RoleAssign)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(RoleAssign)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(RoleAssign)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(RoleAssign)}",
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
					string sql = "SELECT * FROM RoleAssign" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoRoleAssign> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoRoleAssign
					{
						Id = x.Field<int?>("Id"),
						GroupUserId = x.Field<int?>("GroupUserId"),
						FunctId = x.Field<int?>("FunctId"),
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
		public ActionResult<DtoResult<DtoRoleAssign>> Get()
		{
			DtoRoleAssign dto = new();
			DtoResult<DtoRoleAssign> result = GetRoleAssign(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoRoleAssign>> GetOne(DtoRoleAssign RoleAssign)
		{
			DtoResult<DtoRoleAssign> result = GetRoleAssign(RoleAssign, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoRoleAssign>> Find(DtoRoleAssign RoleAssign)
		{
			DtoResult<DtoRoleAssign> result = GetRoleAssign(RoleAssign);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoRoleAssign>> Add(DtoRoleAssign RoleAssign)
		{
			DtoResult<DtoRoleAssign>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("RoleAssignAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@GroupUserId", SqlDbType.Int).Value = RoleAssign.GroupUserId;
					cmd.Parameters.AddWithValue("@FunctId", SqlDbType.Int).Value = RoleAssign.FunctId;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoRoleAssign> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoRoleAssign
						{
							Id = x.Field<int?>("Id"),
							GroupUserId = x.Field<int?>("GroupUserId"),
							FunctId = x.Field<int?>("FunctId"),
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
		public ActionResult<DtoResult<DtoRoleAssign>> Update(DtoRoleAssign RoleAssign)
		{
			DtoResult<DtoRoleAssign>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("RoleAssignUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = RoleAssign.Id;
					cmd.Parameters.AddWithValue("@GroupUserId", SqlDbType.Int).Value = RoleAssign.GroupUserId;
					cmd.Parameters.AddWithValue("@FunctId", SqlDbType.Int).Value = RoleAssign.FunctId;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(RoleAssign);
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
		public ActionResult<DtoResult<DtoRoleAssign>> Delete(DtoRoleAssign RoleAssign)
		{
			DtoResult<DtoRoleAssign>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("RoleAssignDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = RoleAssign.Id;
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