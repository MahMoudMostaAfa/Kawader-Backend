namespace Kawadar.Api.Requests.Job
{
    public class ReviewJobRequest
    {
        public float rating { get; set; }
        public string Comment { get; set; } = "";
        public string RevieweeUserName { get; set; } = "";
    }
}
