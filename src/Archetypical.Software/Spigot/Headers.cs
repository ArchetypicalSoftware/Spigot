using System.Collections.Generic;
using System.Linq;

namespace Archetypical.Software.Spigot
{
    /// <summary>
    /// A collection of headers sent with the data
    /// </summary>
    public class Headers:List<Header>
    {
        /// <summary>
        /// A simple index by name
        /// </summary>
        /// <param name="name">The name of the header</param>
        /// <returns><see cref="Header"/></returns>
        public Header this[string name]
        {
            get { return this.FirstOrDefault(x => x.Name == name); }
        }

    }
}