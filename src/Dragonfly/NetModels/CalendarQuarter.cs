using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonfly.NetModels
{
    /// <summary>
    /// Information representing a Quarter (using calendar quarters)
    /// </summary>
    public class CalendarQuarter
    {
        public int QuarterNumber { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Create a CalendarQuarter using a Quarter Number and a Year
        /// </summary>
        /// <param name="QuarterNumber"></param>
        /// <param name="Year"></param>
        public CalendarQuarter(int QuarterNumber, int Year)
        {
            this.QuarterNumber = QuarterNumber;
            this.Year = Year;
            this.StartDate = new DateTime(Year, (3 * QuarterNumber) - 2, 1);
            this.EndDate = StartDate.AddMonths(3).AddDays(-1);
        }

        /// <summary>
        /// Create a CalendarQuarter using a single date
        /// </summary>
        /// <param name="Date"></param>
        public CalendarQuarter(DateTime Date)
        {
            this.QuarterNumber = (Date.Month + 2) / 3;
            this.Year = Date.Year;
            this.StartDate = new DateTime(Date.Year, (QuarterNumber - 1) * 3 + 1, 1);
            this.EndDate = StartDate.AddMonths(3).AddDays(-1);
        }

        public CalendarQuarter GetPriorQuarter()
        {
            var priorQ = 0;
            var priorY = 0;
            if (this.QuarterNumber == 1)
            {
                priorQ = 4;
                priorY = this.Year-1;
            }
            else
            {
                priorQ = this.QuarterNumber - 1;
                priorY = this.Year;
            }

            return new CalendarQuarter(priorQ,priorY);
        }
    }
}
