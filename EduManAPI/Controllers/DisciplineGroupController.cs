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
	public class DisciplineGroupController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public DisciplineGroupController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoDisciplineGroup> GetDisciplineGroup(DtoDisciplineGroup DisciplineGroup, bool ExactFind = false)
		{
			DtoResult<DtoDisciplineGroup> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in DisciplineGroup.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(DisciplineGroup) != null)
				{
					int index = Array.IndexOf(DisciplineGroup.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += DisciplineGroup.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(DisciplineGroup)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(DisciplineGroup)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(DisciplineGroup)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(DisciplineGroup)}%'",
						};
					else
						condStr += DisciplineGroup.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(DisciplineGroup)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(DisciplineGroup)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(DisciplineGroup)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(DisciplineGroup)}",
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
					string sql = "SELECT * FROM DisciplineGroup" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoDisciplineGroup> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDisciplineGroup
					{
						Id = x.Field<int?>("Id"),
						DisciplineGroupName = x.Field<string?>("DisciplineGroupName"),
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
		public ActionResult<DtoResult<DtoDisciplineGroup>> Get()
		{
			DtoDisciplineGroup dto = new();
			DtoResult<DtoDisciplineGroup> result = GetDisciplineGroup(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoDisciplineGroup>> GetOne(DtoDisciplineGroup DisciplineGroup)
		{
			DtoResult<DtoDisciplineGroup> result = GetDisciplineGroup(DisciplineGroup, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoDisciplineGroup>> Find(DtoDisciplineGroup DisciplineGroup)
		{
			DtoResult<DtoDisciplineGroup> result = GetDisciplineGroup(DisciplineGroup);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoDisciplineGroup>> Add(DtoDisciplineGroup DisciplineGroup)
		{
			DtoResult<DtoDisciplineGroup>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineGroupAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@DisciplineGroupName", SqlDbType.NVarChar).Value = DisciplineGroup.DisciplineGroupName;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoDisciplineGroup> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDisciplineGroup
						{
							Id = x.Field<int?>("Id"),
							DisciplineGroupName = x.Field<string?>("DisciplineGroupName"),
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
		public ActionResult<DtoResult<DtoDisciplineGroup>> Update(DtoDisciplineGroup DisciplineGroup)
		{
			DtoResult<DtoDisciplineGroup>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineGroupUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = DisciplineGroup.Id;
					cmd.Parameters.AddWithValue("@DisciplineGroupName", SqlDbType.NVarChar).Value = DisciplineGroup.DisciplineGroupName;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(DisciplineGroup);
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
		public ActionResult<DtoResult<DtoDisciplineGroup>> Delete(DtoDisciplineGroup DisciplineGroup)
		{
			DtoResult<DtoDisciplineGroup>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineGroupDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = DisciplineGroup.Id;
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