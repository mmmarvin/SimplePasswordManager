namespace SimplePasswordManager.Utility
{
    public static class StringUtility
    {
        public static string ReduceStringLength(string str)
        {
            if(str.Length > 6)
            {
                return str.Substring(0, 6) + "...";
            }

            return str;
        }
    }
}
