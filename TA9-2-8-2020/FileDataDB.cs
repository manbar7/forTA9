using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA9
{
    class FileDataDB
    {
        public int ID { get; set; }
        public string LOCATION_NAME { get; set; }
        public string LONGITUDE { get; set; }
        public string LATITUDE { get; set; }
        public string DATE { get; set; }
        public int DAY { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public int HOUR { get; set; }
        public int MINUTE { get; set; }
        public string DOW { get; set; }
        public string IP { get; set; }

        public FileDataDB()
        {
        }

        public override string ToString()
        {
            return ($"ID:{ID},Location Name:{LOCATION_NAME},Longitude:{LONGITUDE},Latitude:{LATITUDE},Date:{DATE},Day:{DAY},Month:{MONTH},Year:{YEAR},Hour:{HOUR},Minute:{MINUTE},Dow:{DOW},IP:{IP}");
        }
    }
}
