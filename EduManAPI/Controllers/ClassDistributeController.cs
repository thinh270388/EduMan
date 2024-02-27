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
	public class ClassDistributeController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public ClassDistributeController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoClassDistribute> GetClassDistribute(DtoClassDistribute ClassDistribute, bool ExactFind = false)
		{
			DtoResult<DtoClassDistribute> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in ClassDistribute.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(ClassDistribute) != null)
				{
					int index = Array.IndexOf(ClassDistribute.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += ClassDistribute.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(ClassDistribute)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(ClassDistribute)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(ClassDistribute)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(ClassDistribute)}%'",
						};
					else
						condStr += ClassDistribute.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(ClassDistribute)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(ClassDistribute)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(ClassDistribute)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(ClassDistribute)}",
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
					string sql = "SELECT * FROM ClassDistribute" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoClassDistribute> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoClassDistribute
					{
						Id = x.Field<int?>("Id"),
						ClassId = x.Field<int?>("ClassId"),
						TeacherId = x.Field<int?>("TeacherId"),
						AssignDate = x.Field<DateTime?>("AssignDate"),
						OnYear = x.Field<string?>("OnYear"),
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
		public ActionResult<DtoResult<DtoClassDistribute>> Get()
		{
			DtoClassDistribute dto = new();
			DtoResult<DtoClassDistribute> result = GetClassDistribute(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoClassDistribute>> GetOne(DtoClassDistribute ClassDistribute)
		{
			DtoResult<DtoClassDistribute> result = GetClassDistribute(ClassDistribute, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoClassDistribute>> Find(DtoClassDistribute ClassDistribute)
		{
			DtoResult<DtoClassDistribute> result = GetClassDistribute(ClassDistribute);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoClassDistribute>> Add(DtoClassDistribute ClassDistribute)
		{
			DtoResult<DtoClassDistribute>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDistributeAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@ClassId", SqlDbType.Int).Value = ClassDistribute.ClassId;
					cmd.Parameters.AddWithValue("@TeacherId", SqlDbType.Int).Value = ClassDistribute.TeacherId;
					cmd.Parameters.AddWithValue("@AssignDate", SqlDbType.Date).Value = ClassDistribute.AssignDate;
					cmd.Parameters.AddWithValue("@OnYear", SqlDbType.VarChar).Value = ClassDistribute.OnYear;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = ClassDistribute.Note == null ? DBNull.Value : ClassDistribute.Note;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoClassDistribute> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoClassDistribute
						{
							Id = x.Field<int?>("Id"),
							ClassId = x.Field<int?>("ClassId"),
							TeacherId = x.Field<int?>("TeacherId"),
							AssignDate = x.Field<DateTime?>("AssignDate"),
							OnYear = x.Field<string?>("OnYear"),
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
		public ActionResult<DtoResult<DtoClassDistribute>> Update(DtoClassDistribute ClassDistribute)
		{
			DtoResult<DtoClassDistribute>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDistributeUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = ClassDistribute.Id;
					cmd.Parameters.AddWithValue("@ClassId", SqlDbType.Int).Value = ClassDistribute.ClassId;
					cmd.Parameters.AddWithValue("@TeacherId", SqlDbType.Int).Value = ClassDistribute.TeacherId;
					cmd.Parameters.AddWithValue("@AssignDate", SqlDbType.Date).Value = ClassDistribute.AssignDate;
					cmd.Parameters.AddWithValue("@OnYear", SqlDbType.VarChar).Value = ClassDistribute.OnYear;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = ClassDistribute.Note == null ? DBNull.Value : ClassDistribute.Note;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(ClassDistribute);
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
		public ActionResult<DtoResult<DtoClassDistribute>> Delete(DtoClassDistribute ClassDistribute)
		{
			DtoResult<DtoClassDistribute>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDistributeDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = ClassDistribute.Id;
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