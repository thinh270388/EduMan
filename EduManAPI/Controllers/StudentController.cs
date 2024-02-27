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
	public class StudentController : ControllerBase
	{
		private readonly Encryption encryption = new();
		private readonly SqlConnection conn = new();
		public StudentController()
		{
			conn = new($"Data Source={encryption.Decrypt(Admin.serverip, Admin.key)};Initial Catalog=EduMan;Encrypt=false;Persist Security Info=True;User ID={encryption.Decrypt(Admin.user, Admin.key)};Password={encryption.Decrypt(Admin.pass, Admin.key)}");
		}
		private DtoResult<DtoStudent> GetStudent(DtoStudent Student, bool ExactFind = false)
		{
			DtoResult<DtoStudent> result = new();
			string condStr = "";
			Type[] typeInQuote = { typeof(bool), typeof(bool?), typeof(DateTime), typeof(DateTime?) };
			foreach (PropertyInfo prop in Student.GetType().GetProperties())
			{
				if (prop.Name == "TypeList")
					continue;
				if (prop.GetValue(Student) != null)
				{
					int index = Array.IndexOf(Student.GetType().GetProperties(), prop);
					if(!ExactFind)
						condStr += Student.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} LIKE '%{prop.GetValue(Student)}%'",
							"nvarchar" => $" AND {prop.Name} LIKE N'%{prop.GetValue(Student)}%'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Student)}'",
							_ => $" AND {prop.Name} LIKE '%{prop.GetValue(Student)}%'",
						};
					else
						condStr += Student.TypeList[index] switch
						{
							"varchar" => $" AND {prop.Name} = '{prop.GetValue(Student)}'",
							"nvarchar" => $" AND {prop.Name} = N'{prop.GetValue(Student)}'",
							"bit" or "date" or "datetime" => $" AND {prop.Name} = '{prop.GetValue(Student)}'",
							_ => $" AND {prop.Name} = {prop.GetValue(Student)}",
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
					string sql = "SELECT * FROM Student" + condStr;
					SqlDataAdapter adapter = new(sql, conn);
					DataTable dt = new();
 					adapter.Fill(dt);
					conn.Close();
					List<DtoStudent> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStudent
					{
						Id = x.Field<int?>("Id"),
						Code = x.Field<string?>("Code"),
						FullName = x.Field<string?>("FullName"),
						Birthday = x.Field<DateTime?>("Birthday"),
						Gender = x.Field<bool?>("Gender"),
						Phone = x.Field<string?>("Phone"),
						Email = x.Field<string?>("Email"),
						AddressCurrent = x.Field<string?>("AddressCurrent"),
						ContactInfo = x.Field<string?>("ContactInfo"),
						SequenceNumber = x.Field<int?>("SequenceNumber"),
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
		public ActionResult<DtoResult<DtoStudent>> Get()
		{
			DtoStudent dto = new();
			DtoResult<DtoStudent> result = GetStudent(dto);
			if(result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}
		[HttpPost("GetOne")]
		public ActionResult<DtoResult<DtoStudent>> GetOne(DtoStudent Student)
		{
			DtoResult<DtoStudent> result = GetStudent(Student, true);
			if (result.Message == "OK")
				return Ok(result);
			else
				return NotFound(result);
		}

		[HttpPost("Find")]
		public ActionResult<List<DtoStudent>> Find(DtoStudent Student)
		{
			DtoResult<DtoStudent> result = GetStudent(Student);
			return Ok(result);
		}

		[HttpPost("Add")]
		public ActionResult<DtoResult<DtoStudent>> Add(DtoStudent Student)
		{
			DtoResult<DtoStudent>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentAdd", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Code", SqlDbType.VarChar).Value = Student.Code;
					cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = Student.FullName;
					cmd.Parameters.AddWithValue("@Birthday", SqlDbType.Date).Value = Student.Birthday == null ? DBNull.Value : Student.Birthday;
					cmd.Parameters.AddWithValue("@Gender", SqlDbType.Bit).Value = Student.Gender == null ? DBNull.Value : Student.Gender;
					cmd.Parameters.AddWithValue("@Phone", SqlDbType.NVarChar).Value = Student.Phone == null ? DBNull.Value : Student.Phone;
					cmd.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = Student.Email == null ? DBNull.Value : Student.Email;
					cmd.Parameters.AddWithValue("@AddressCurrent", SqlDbType.NVarChar).Value = Student.AddressCurrent == null ? DBNull.Value : Student.AddressCurrent;
					cmd.Parameters.AddWithValue("@ContactInfo", SqlDbType.NVarChar).Value = Student.ContactInfo == null ? DBNull.Value : Student.ContactInfo;
					cmd.Parameters.AddWithValue("@SequenceNumber", SqlDbType.Int).Value = Student.SequenceNumber == null ? DBNull.Value : Student.SequenceNumber;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = Student.Note == null ? DBNull.Value : Student.Note;
					conn.Open();
					SqlDataAdapter adapt = new(cmd);
					DataTable dt = new();
					adapt.Fill(dt);
					conn.Close();
					if (dt.Rows.Count>0)
					{
						List<DtoStudent> rs = dt.Rows.Cast<DataRow>().ToList().Select(x => new DtoStudent
						{
							Id = x.Field<int?>("Id"),
							Code = x.Field<string?>("Code"),
							FullName = x.Field<string?>("FullName"),
							Birthday = x.Field<DateTime?>("Birthday"),
							Gender = x.Field<bool?>("Gender"),
							Phone = x.Field<string?>("Phone"),
							Email = x.Field<string?>("Email"),
							AddressCurrent = x.Field<string?>("AddressCurrent"),
							ContactInfo = x.Field<string?>("ContactInfo"),
							SequenceNumber = x.Field<int?>("SequenceNumber"),
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
		public ActionResult<DtoResult<DtoStudent>> Update(DtoStudent Student)
		{
			DtoResult<DtoStudent>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentUpdate", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Student.Id;
					cmd.Parameters.AddWithValue("@Code", SqlDbType.VarChar).Value = Student.Code;
					cmd.Parameters.AddWithValue("@FullName", SqlDbType.NVarChar).Value = Student.FullName;
					cmd.Parameters.AddWithValue("@Birthday", SqlDbType.Date).Value = Student.Birthday == null ? DBNull.Value : Student.Birthday;
					cmd.Parameters.AddWithValue("@Gender", SqlDbType.Bit).Value = Student.Gender == null ? DBNull.Value : Student.Gender;
					cmd.Parameters.AddWithValue("@Phone", SqlDbType.NVarChar).Value = Student.Phone == null ? DBNull.Value : Student.Phone;
					cmd.Parameters.AddWithValue("@Email", SqlDbType.NVarChar).Value = Student.Email == null ? DBNull.Value : Student.Email;
					cmd.Parameters.AddWithValue("@AddressCurrent", SqlDbType.NVarChar).Value = Student.AddressCurrent == null ? DBNull.Value : Student.AddressCurrent;
					cmd.Parameters.AddWithValue("@ContactInfo", SqlDbType.NVarChar).Value = Student.ContactInfo == null ? DBNull.Value : Student.ContactInfo;
					cmd.Parameters.AddWithValue("@SequenceNumber", SqlDbType.Int).Value = Student.SequenceNumber == null ? DBNull.Value : Student.SequenceNumber;
					cmd.Parameters.AddWithValue("@Note", SqlDbType.NVarChar).Value = Student.Note == null ? DBNull.Value : Student.Note;
					conn.Open();
					int count = cmd.ExecuteNonQuery();
					conn.Close();
					if (count > 0)
					{
						return GetOne(Student);
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
		public ActionResult<DtoResult<DtoStudent>> Delete(DtoStudent Student)
		{
			DtoResult<DtoStudent>? result = new();
			try
			{
				using (conn)
				{
					using SqlCommand cmd = new("StudentDelete", conn) { CommandType = CommandType.StoredProcedure };
					cmd.Parameters.AddWithValue("@Id", SqlDbType.Int).Value = Student.Id;
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