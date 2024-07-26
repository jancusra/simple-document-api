using System.Collections.Generic;

namespace App.Contracts.Models
{
    public partial class DocumentDto : BaseDto
    {
        public DocumentDto()
        {
            data = new Dictionary<string, string>();
        }

        public string[] tags { get; set; }

        public IDictionary<string, string> data { get; set; }
    }
}