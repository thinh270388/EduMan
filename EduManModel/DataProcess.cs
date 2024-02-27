using EduManModel.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace EduManModel
{
    public partial class DataProcess<T>
    {
        HttpClient client = new();
        public async Task<DtoResult<T>> GetAllAsync(T dto)
        {
            DtoResult<T> result = new();
            string url = UrlGetAll[dto!.GetType()];
            string responseContent;
            try
            {
                var response = await client.GetAsync(url);
                responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    result = JsonConvert.DeserializeObject<DtoResult<T>>(responseContent)!;
                }
                else
                {
                    result.Message = "Không thể kết nối máy chủ";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        public async Task<DtoResult<T>> GetOneAsync(T dto)
        {
            DtoResult<T> result = new();
            string url = UrlGetOne[dto!.GetType()];
            string responseContent;
            try
            {
                string json = JsonConvert.SerializeObject(dto);
                StringContent content = new(json, Encoding.UTF8, "text/json");
                var response = await client.PostAsync(url, content);
                responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    result = JsonConvert.DeserializeObject<DtoResult<T>>(responseContent)!;
                }
                else
                {
                    result.Message = "Không thể kết nối máy chủ";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        public async Task<DtoResult<T>> FindAsync(T dto)
        {
            DtoResult<T> result = new();
            string url = UrlFind[dto!.GetType()];
            string responseContent;
            try
            {
                string json = JsonConvert.SerializeObject(dto);
                StringContent content = new(json, Encoding.UTF8, "text/json");
                var response = await client.PostAsync(url, content);
                responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    result = JsonConvert.DeserializeObject<DtoResult<T>>(responseContent)!;
                }
                else
                {
                    result.Message = "Không thể kết nối máy chủ";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        public async Task<DtoResult<T>> AddAsync(T dto)
        {
            DtoResult<T> result = new();
            string url = UrlAdd[dto!.GetType()];
            string responseContent;
            try
            {
                string json = JsonConvert.SerializeObject(dto);
                StringContent content = new(json, Encoding.UTF8, "text/json");
                var response = await client.PostAsync(url, content);
                responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    result = JsonConvert.DeserializeObject<DtoResult<T>>(responseContent)!;
                }
                else
                {
                    result.Message = "Không thể kết nối máy chủ";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        public async Task<DtoResult<T>> UpdateAsync(T dto)
        {
            DtoResult<T> result = new();
            string url = UrlUpdate[dto!.GetType()];
            string responseContent;
            try
            {
                string json = JsonConvert.SerializeObject(dto);
                StringContent content = new(json, Encoding.UTF8, "text/json");
                var response = await client.PostAsync(url, content);
                responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    result = JsonConvert.DeserializeObject<DtoResult<T>>(responseContent)!;
                }
                else
                {
                    result.Message = "Không thể kết nối máy chủ";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
        public async Task<DtoResult<T>> DeleteAsync(T dto)
        {
            DtoResult<T> result = new();
            string url = UrlDelete[dto!.GetType()];
            string responseContent;
            try
            {
                string json = JsonConvert.SerializeObject(dto);
                StringContent content = new(json, Encoding.UTF8, "text/json");
                var response = await client.PostAsync(url, content);
                responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    result = JsonConvert.DeserializeObject<DtoResult<T>>(responseContent)!;
                }
                else
                {
                    result.Message = "Không thể kết nối máy chủ";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
            return result;
        }
    }
}
