﻿namespace openPER.Models
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
    }
}
