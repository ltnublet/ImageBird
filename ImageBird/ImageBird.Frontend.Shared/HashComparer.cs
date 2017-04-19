using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBird.Frontend.Shared
{
    /// <summary>
    /// Handles comparing hashes.
    /// </summary>
    public class HashComparer
    {
        /// <summary>
        /// The number of characters to match to when comparing hashes.
        /// </summary>
        public int Characters { get; private set; }

        /// <summary>
        /// Instantiates a new <see cref="HashComparer"/> using the supplied parameters.
        /// </summary>
        /// <param name="characters">
        /// The number of characters to match to when comparing hashes.
        /// </param>
        public HashComparer(int characters)
        {
            this.Characters = characters;
        }

        /// <summary>
        /// Compares the two hashes.
        /// </summary>
        /// <param name="left">
        /// The first hash to compare.
        /// </param>
        /// <param name="right">
        /// The second hash to compare.
        /// </param>
        /// <returns>
        /// True if the supplied hashes matched given this <see cref="HashComparer"/>'s state, and false otherwise.
        /// </returns>
        public bool Compare(string left, string right)
        {
            throw new NotImplementedException();
        }
    }
}
