namespace StudentAutomationMVC.Web.Models.Shared
{
    public class ApiDataResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

}
