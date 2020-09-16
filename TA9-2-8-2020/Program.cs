using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using LumenWorks.Framework.IO.Csv;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace TA9
{
    class Program
    {

        static List<FileData> DataFiles = new List<FileData>();
        static List<FileDataDB> DataFilesDB = new List<FileDataDB>();
        public static SqlCommand cmd = new SqlCommand();


        public static string ReadFromUrl(string url)
        {

            string read_str = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            // read data
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                read_str = reader.ReadToEnd();
            }

            return read_str;
        }

        private static void ReadCsvFile()
        {

            var csvTable = new DataTable();
           using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"C:\FileData.csv")), true))
            {
                csvTable.Load(csvReader);
            }
            for (int i = 0; i < csvTable.Rows.Count; i++)
            {
                //   string date = DataFiles[i].Date.ToString();
                //  DataFiles.Add(new FileData { Date = csvTable.Rows[i][0].ToString(), LocationName = csvTable.Rows[i][1].ToString(), IP = csvTable.Rows[i][2].ToString() });
                DataFiles.Add(new FileData { Date = csvTable.Rows[i][0].ToString(), LocationName = csvTable.Rows[i][1].ToString(), IP = csvTable.Rows[i][2].ToString() });
                Console.WriteLine(DataFiles.ToString());
                //GetCoordinatesLocationByIP(DataFiles[i]);

            }

        }

        private static FileDataDB ConfigureList(FileData fileData,int a)
        {
           //nt counter = 1;
            FileDataDB fd = new FileDataDB();
            List<FileDataDB> FileDatasDB = new List<FileDataDB>();
            //for (int i = 0; i < fileData.Count; i++)
            // {
                string iString = fileData.Date;
                DateTime oDate = Convert.ToDateTime(iString);

                fd.ID = a;
                fd.LOCATION_NAME = fileData.LocationName;
                fd.DATE = oDate.Date.ToString();
                fd.DAY = Convert.ToInt32(oDate.Day);
                fd.MONTH = Convert.ToInt32(oDate.Month);
                fd.YEAR = Convert.ToInt32(oDate.Year);
                fd.HOUR = Convert.ToInt32(oDate.Hour);
                fd.MINUTE = Convert.ToInt32(oDate.Minute);
                fd.DOW = oDate.DayOfWeek.ToString();
                fd.IP = fileData.IP;
                fd = ConfigIP(fd);
           //     FileDatasDB.Add(fd);
            // }

            return fd;

        }

        private static FileDataDB ConfigIP(FileDataDB filedata)
        {         
                string ip = filedata.IP;
                bool IPstate = IPCheck(ip);
                if (IPstate == false)
                {
                
                    filedata.IP = null;
                 
                }    
            return filedata;
        }

        private static List<FileDataDB> EnrichWithCoords(List<FileDataDB> filedatas)
        {

            string url = "https://api.ipgeolocationapi.com/geolocate/";



            for (int i = 0; i < filedatas.Count; i++)
            {
                // var obj = new { IP = "79.178.108.27" };
                var obj = filedatas[i].IP;
                string result = ReadFromUrl(url + obj);
                JObject data = JObject.Parse(result);
                JObject geo = JObject.Parse(data.GetValue("geo").ToString());
                string latitude = geo.GetValue("latitude").ToString();
                string longitude = geo.GetValue("longitude").ToString();
                filedatas[i].LATITUDE = latitude;
                filedatas[i].LONGITUDE = longitude;
                
            }

            return filedatas;
        }


        static FileDataDB ConnectToDB(FileDataDB f)
        {
            using (cmd.Connection = new SqlConnection(@"Data Source=.;Initial Catalog=TA9DB;Integrated Security=True"))
            {
                cmd.Connection.Open();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = $"INSERT INTO DataFiles " + $" (ID,LOCATION_NAME,LONGITUDE,LATITUDE,DATE,DAY,MONTH,YEAR,HOUR,MINUTE,DOW,IP) " + $"Values ('{f.ID}','{f.LOCATION_NAME}','{f.LONGITUDE}','{f.LATITUDE}','{f.DATE}','{f.DAY}','{f.MONTH}','{f.YEAR}','{f.HOUR}','{f.MINUTE}','{f.DOW}','{f.IP}' )";
                cmd.ExecuteNonQuery();
                Console.WriteLine(f);
                return f;
            }

        }


        private static bool IPCheck(string ip)
        {
            char chrFullStop = '.';
            string[] arrOctets = ip.Split(chrFullStop);
            if (arrOctets.Length != 4)
            {
                return false;
            }
            Int16 MAXVALUE = 255;
            Int32 temp;
            foreach (String strOctet in arrOctets)
            {
                if (strOctet.Length > 3)
                {
                    return false;
                }

                temp = int.Parse(strOctet);
                if (temp > MAXVALUE)
                {
                    return false;
                }
            }
            return true;
        }




        static void Main(string[] args)
        {
            ReadCsvFile();
            

            FileDataDB f = new FileDataDB();
            //   FileDataDB a = ConnectToDB(f);
            int a = 1;
           // DataFilesDB = ConfigureList(DataFiles);
            foreach (FileData fd in DataFiles)
            {
                
                Console.WriteLine(fd);
                f = ConfigureList(fd,a);
                ConnectToDB(f);
                DataFilesDB.Add(f);
                a++;
            }
           //  ConfigIP(DataFilesDB);
           // EnrichWithCoords(DataFilesDB);
             /*
            foreach (FileDataDB fd in DataFilesDB)
            {
                FileDataDB a = ConnectToDB(fd);
                DataFilesDB.Add(fd);
              //  DataFilesDB.Add(a);
                Console.WriteLine(fd);
            }                  
            */
            Console.ReadLine();
        }




        



    }
}
