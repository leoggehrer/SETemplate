//@BaseCode
namespace SETemplate.WebApi.Models
{
    public partial class QueryParams
    {
        public string Filter { get; set; } = string.Empty;
        public string[] Values { get; set; } = [];
        public string[] Includes { get; set; } = [];
    }
}
