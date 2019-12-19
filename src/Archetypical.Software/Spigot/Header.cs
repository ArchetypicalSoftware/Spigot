namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// A named value that can hold custom information passed with each message
    /// </summary>
    public class Header
    {
        public Header()
        {
        }

        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// The name of the Header
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of the Header
        /// </summary>
        public string Value { get; set; }
    }
}