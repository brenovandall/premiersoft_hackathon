namespace Hackathon.Premiersoft.API.Dto
{
    public class FileXmlDto
    {
        public long LineNumber { get; set; }
        public string Tag { get; set; }
        public string Value { get; set; }

        public FileXmlDto(long lineNumber, string tag, string value) {
        
            LineNumber = lineNumber;
            Tag = tag;
            Value = value;  
        }
    }
}
