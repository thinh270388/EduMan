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
	public class DisciplineTypeController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public DisciplineTypeController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoDisciplineType> GetDisciplineType(DtoDisciplineType DisciplineType, bool ExactFind = false)
		{
			DtoResult<DtoDisciplineType> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in DisciplineType.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(DisciplineType) != null)
				{
					int index = Array.IndexOf(DisciplineType.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += DisciplineType.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(DisciplineType)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(DisciplineType)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(DisciplineType)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(DisciplineType)}%'",
						};
					else
						condStr += DisciplineType.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(DisciplineType)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(DisciplineType)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(DisciplineType)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(DisciplineType)}",
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
					string sql = "SELECT * FROM DisciplineType" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoDisciplineType> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDisciplineType
					{
						Id = x.Field<int?>("Id"),
						DisciplineTypeName = x.Field<string?>("DisciplineTypeName"),
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
		public ActionResult<DtoResult<DtoDisciplineType>> Get()
		{
			DtoDisciplineType dto = new();
			DtoResult<DtoDisciplineType> result = GetDisciplineType(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoDisciplineType>> GetOne(DtoDisciplineType DisciplineType)
		{
			DtoResult<DtoDisciplineType> result = GetDisciplineType(DisciplineType, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoDisciplineType>> Find(DtoDisciplineType DisciplineType)
		{
			DtoResult<DtoDisciplineType> result = GetDisciplineType(DisciplineType);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoDisciplineType>> Add(DtoDisciplineType DisciplineType)
		{
			DtoResult<DtoDisciplineType>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineTypeAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@DisciplineTypeName", SqlDbType.NVarChar).Value = DisciplineType.DisciplineTypeName;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoDisciplineType> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoDisciplineType
						{
							Id = x.Field<int?>("Id"),
							DisciplineTypeName = x.Field<string?>("DisciplineTypeName"),
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
		public ActionResult<DtoResult<DtoDisciplineType>> Update(DtoDisciplineType DisciplineType)
		{
			DtoResult<DtoDisciplineType>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineTypeUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = DisciplineType.Id;
					cmd.Parameters.AddWithValue("@DisciplineTypeName", SqlDbType.NVarChar).Value = DisciplineType.DisciplineTypeName;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(DisciplineType);
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
		public ActionResult<DtoResult<DtoDisciplineType>> Delete(DtoDisciplineType DisciplineType)
		{
			DtoResult<DtoDisciplineType>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("DisciplineTypeDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = DisciplineType.Id;
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