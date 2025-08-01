namespace CMS.Api.Models
{
    public class BaseBatch
    {
        public bool IsValid => !Messages.Any();
        private ICollection<string> Messages { get; set; } = new List<string>();
        public void AddMessage(string message)
        {
            Messages.Add(message);
        }

        public void OverrideMessage(string message) { 
        
            Messages.Clear();
            Messages.Add(message);
        }
        public string GetMessages()
        {
            return string.Join(string.Empty,Messages.ToArray());
        }
    }
}
