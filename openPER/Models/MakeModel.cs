namespace openPER.Models
{
    public class MakeModel
    {
        public MakeModel(string code, string description)
        {
            Code = code;
            Description = description;
        }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
