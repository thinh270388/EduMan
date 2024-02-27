namespace EduManModel.Dtos
{
    public class DtoResult<T>
    {
        public T? Result { get; set; }
        public List<T>? Results { get; set; }
        public string? Message { get; set; }
    }
}