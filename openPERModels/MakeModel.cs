using System.Collections.Generic;

namespace openPERModels
{
    public class MakeModel
    {
        public MakeModel(string code,string subCode, string description)
        {
            Code = code;
            Description = description;
            SubCode = subCode;  
        }
        public string Code { get; set; }
        public string SubCode { get; set; }
        public string Description { get; set; }
        public List<ModelModel> Models { get; set; }
    }
}
