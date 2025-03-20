//@BaseCode
//MdStart
namespace TemplateTools.ConApp.Models
{
    public partial class ReferenceItem
    {
        public string Tag { get; set; } = string.Empty;
        public required string Name { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Info { get; set; } = string.Empty;
        public List<ReferenceItem> Childs { get; } = [];
        public override string ToString()
        {
            var result = new System.Text.StringBuilder();

            result.AppendLine($"{Name} - {Reference}");

            foreach (var item in Childs)
            {
                result.AppendLine($"  -> {item.Tag} - {item.Reference}");
            }
            return result.ToString();
        }
    }
}
//MdEnd