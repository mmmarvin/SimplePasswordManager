using System.Collections.Generic;

namespace SimplePasswordManager
{
    public class LoadEvent
    {
        public Dictionary<string, List<UsernamePassword>> UsernamePasswords { get; set; }
    }

    public class PasswordEvent
    {
        public string Category { get; set; }
        public UsernamePassword UsernamePassword { get; set; }
    }

    public class PasswordUpdateEvent : PasswordEvent
    {
        public int Index { get; set; }
    }

    public class CategoryEvent
    {
        public string Category { get; set; }
    }   
}
