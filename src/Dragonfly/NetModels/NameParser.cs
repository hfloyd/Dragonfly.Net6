namespace Dragonfly.NetModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class NameParser
    {
        public string Prefix { get;  set; }
        public string First { get;  set; }
        public string Middle { get;  set; }
        public string Last { get;  set; }
        public string Suffix { get;  set; }
        public string OriginalFullName { get; internal set; }

        public NameParser(string FullName)
        {
            this.OriginalFullName = FullName;
            ParseName();
        }

        private void ParseName()
        {
            this.Prefix = "";
            this.First = "";
            this.Middle = "";
            this.Last = "";
            this.Suffix = "";

            // Split on period, commas or spaces, but don't remove from results.
            List<string> parts = Regex.Split(this.OriginalFullName, @"(?<=[., ])").ToList();

            // Remove any empty parts
            for (int x = parts.Count - 1; x >= 0; x--)
                if (parts[x].Trim() == "")
                    parts.RemoveAt(x);

            if (parts.Count > 0)
            {
                // Might want to add more to this list
                string[] prefixes = { "mr", "mrs", "ms", "dr", "miss", "sir", "madam", "mayor", "president" };

                // If first part is a prefix, set prefix and remove part
                string normalizedPart = parts.First().Replace(".", "").Replace(",", "").Trim().ToLower();
                if (prefixes.Contains(normalizedPart))
                {
                    this.Prefix = parts[0].Trim();
                    parts.RemoveAt(0);
                }
            }

            if (parts.Count > 0)
            {
                // Might want to add more to this list, or use code/regex for roman-numeral detection
                string[] suffixes = { "jr", "sr", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix", "x", "xi", "xii", "xiii", "xiv", "xv" };

                // If last part is a suffix, set suffix and remove part
                string normalizedPart = parts.Last().Replace(".", "").Replace(",", "").Trim().ToLower();
                if (suffixes.Contains(normalizedPart))
                {
                    this.Suffix = parts.Last().Replace(",", "").Trim();
                    parts.RemoveAt(parts.Count - 1);
                }
            }

            // Done, if no more parts
            if (parts.Count == 0)
                return;

            // If only one part left...
            if (parts.Count == 1)
            {
                // If no prefix, assume first name, otherwise last
                // i.e.- "Dr Jones", "Ms Jones" -- likely to be last
                if (this.Prefix == "")
                    this.First = parts.First().Replace(",", "").Trim();
                else
                    this.Last = parts.First().Replace(",", "").Trim();
            }

            // If first part ends with a comma, assume format:
            //   Last, First [...First...]
            else if (parts.First().EndsWith(","))
            {
                this.Last = parts.First().Replace(",", "").Trim();
                for (int x = 1; x < parts.Count; x++)
                    this.First += parts[x].Replace(",", "").Trim() + " ";
                this.First = this.First.Trim();
            }

            // Otherwise assume format:
            // First [...Middle...] Last

            else
            {
                this.First = parts.First().Replace(",", "").Trim();
                this.Last = parts.Last().Replace(",", "").Trim();
                for (int x = 1; x < parts.Count - 1; x++)
                    this.Middle += parts[x].Replace(",", "").Trim() + " ";
                this.Middle = this.Middle.Trim();
            }
        }
    }

}
