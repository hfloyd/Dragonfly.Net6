namespace Dragonfly.NetHelpers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public static class Culture
    {
        private const string ThisClassName = "Dragonfly.NetHelpers.Culture";

        #region Countries

        public static string GetCountryName(string Abbreviation)
        {
            var countryName = "";
            var allCountries = GetAllCountries();

            if (Abbreviation.Length == 2)
            {
                var match = allCountries.Where(c => c.TwoLetterISORegionName == Abbreviation).FirstOrDefault();
                countryName = match != null ? match.EnglishName : "";
            }
            else if (Abbreviation.Length == 3)
            {
                var match = allCountries.Where(c => c.ThreeLetterISORegionName == Abbreviation).FirstOrDefault();
                if (match != null)
                {
                    countryName = match.EnglishName;
                }
                else
                {
                    match = allCountries.Where(c => c.ThreeLetterWindowsRegionName == Abbreviation).FirstOrDefault();
                    countryName = match != null ? match.EnglishName : "";
                }
            }

            return countryName;
        }

        public static IEnumerable<RegionInfo> GetAllCountries()
        {
            var countryList = new SortedDictionary<string, RegionInfo>();

            // Iterate the Framework Cultures...
            foreach (CultureInfo ci in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                RegionInfo ri = null;

                try
                {
                    ri = new RegionInfo(ci.Name);
                }
                catch
                {
                    // If a RegionInfo object could not be created we don't want to use the CultureInfo
                    //    for the country list.
                    continue;
                }

                // Create new country dictionary entry.
                var newKeyValuePair = new KeyValuePair<string, RegionInfo>(ri.EnglishName, ri);

                // If the country is not already in the countryList add it...
                if (!(countryList.ContainsKey(ri.EnglishName)))
                {
                    countryList.Add(newKeyValuePair.Key, newKeyValuePair.Value);
                }
            }

            return countryList.Select(x => x.Value);
        }

        #endregion
    }
}
