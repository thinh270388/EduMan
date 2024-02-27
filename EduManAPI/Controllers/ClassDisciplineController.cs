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
	public class ClassDisciplineController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public ClassDisciplineController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoClassDiscipline> GetClassDiscipline(DtoClassDiscipline ClassDiscipline, bool ExactFind = false)
		{
			DtoResult<DtoClassDiscipline> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in ClassDiscipline.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(ClassDiscipline) != null)
				{
					int index = Array.IndexOf(ClassDiscipline.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += ClassDiscipline.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(ClassDiscipline)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(ClassDiscipline)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(ClassDiscipline)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(ClassDiscipline)}%'",
						};
					else
						condStr += ClassDiscipline.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(ClassDiscipline)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(ClassDiscipline)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(ClassDiscipline)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(ClassDiscipline)}",
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
					string sql = "SELECT * FROM ClassDiscipline" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoClassDiscipline> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoClassDiscipline
					{
						Id = x.Field<int?>("Id"),
						ClassDistributeId = x.Field<int?>("ClassDistributeId"),
						DisciplineId = x.Field<int?>("DisciplineId"),
						WeeklyId = x.Field<int?>("WeeklyId"),
						OnDate = x.Field<DateTime?>("OnDate"),
						Times = x.Field<int?>("Times"),
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
		public ActionResult<DtoResult<DtoClassDiscipline>> Get()
		{
			DtoClassDiscipline dto = new();
			DtoResult<DtoClassDiscipline> result = GetClassDiscipline(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoClassDiscipline>> GetOne(DtoClassDiscipline ClassDiscipline)
		{
			DtoResult<DtoClassDiscipline> result = GetClassDiscipline(ClassDiscipline, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoClassDiscipline>> Find(DtoClassDiscipline ClassDiscipline)
		{
			DtoResult<DtoClassDiscipline> result = GetClassDiscipline(ClassDiscipline);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoClassDiscipline>> Add(DtoClassDiscipline ClassDiscipline)
		{
			DtoResult<DtoClassDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDisciplineAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@ClassDistributeId", SqlDbType.Int).Value = ClassDiscipline.ClassDistributeId;
					cmd.Parameters.AddWithValue("@DisciplineId", SqlDbType.Int).Value = ClassDiscipline.DisciplineId;
					cmd.Parameters.AddWithValue("@WeeklyId", SqlDbType.Int).Value = ClassDiscipline.WeeklyId;
					cmd.Parameters.AddWithValue("@OnDate", SqlDbType.Date).Value = ClassDiscipline.OnDate;
					cmd.Parameters.AddWithValue("@Times", SqlDbType.Int).Value = ClassDiscipline.Times;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoClassDiscipline> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoClassDiscipline
						{
							Id = x.Field<int?>("Id"),
							ClassDistributeId = x.Field<int?>("ClassDistributeId"),
							DisciplineId = x.Field<int?>("DisciplineId"),
							WeeklyId = x.Field<int?>("WeeklyId"),
							OnDate = x.Field<DateTime?>("OnDate"),
							Times = x.Field<int?>("Times"),
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
		public ActionResult<DtoResult<DtoClassDiscipline>> Update(DtoClassDiscipline ClassDiscipline)
		{
			DtoResult<DtoClassDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDisciplineUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = ClassDiscipline.Id;
					cmd.Parameters.AddWithValue("@ClassDistributeId", SqlDbType.Int).Value = ClassDiscipline.ClassDistributeId;
					cmd.Parameters.AddWithValue("@DisciplineId", SqlDbType.Int).Value = ClassDiscipline.DisciplineId;
					cmd.Parameters.AddWithValue("@WeeklyId", SqlDbType.Int).Value = ClassDiscipline.WeeklyId;
					cmd.Parameters.AddWithValue("@OnDate", SqlDbType.Date).Value = ClassDiscipline.OnDate;
					cmd.Parameters.AddWithValue("@Times", SqlDbType.Int).Value = ClassDiscipline.Times;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(ClassDiscipline);
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
		public ActionResult<DtoResult<DtoClassDiscipline>> Delete(DtoClassDiscipline ClassDiscipline)
		{
			DtoResult<DtoClassDiscipline>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("ClassDisciplineDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = ClassDiscipline.Id;
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