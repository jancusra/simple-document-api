namespace App.Contracts.Models
{
    public class DocumentDto : BaseDto
    {
        public string[] tags { get; set; }

        public object data { get; set; }
    }
}
