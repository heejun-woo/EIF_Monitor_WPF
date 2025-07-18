using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIF_Monitor_WPF
{
    public class EIF_INFO
    {
        public string ELMNO { get; set; }
        public DateTime DTTM { get; set; }
        public string ELMNAME { get; set; }
        public string LOGIC { get; set; }

        public bool DRIVER_CONNECTED { get; set; }
        public string EQP_STAT { get; set; }
        public bool LOT_RUNNING { get; set; }
        public string LOT_ID { get; set; }
        public bool DRY_RUN { get; set; }
        public bool IT_PASS { get; set; }

        public string HOSTNAME { get; set; }
        public string HOSTIP { get; set; }
        public string BIZIP { get; set; }

        public float CPU_USAGE { get; set; }
        public ulong RAM_TOTAL { get; set; }
        public ulong RAM_USED { get; set; }
        public ulong DISK_TOTAL { get; set; }
        public ulong DISK_USED { get; set; }

        public int PROCID { get; set; }
        public string PROCNAME { get; set; }

        List<string> lstIp = new List<string>();

        public EIF_INFO(string elmno, string dttm, string eifdata, string sysydata, string elmname, string logic, List<string> lstIP)
        {
            ELMNO = elmno;
            DTTM = DateTime.ParseExact(dttm, "yyyyMMddHHmmss", null);

            List<string> lstEIF = eifdata.Trim().Split(':').ToList();
            DRIVER_CONNECTED = lstEIF[0].Trim().Equals("True") ? true : false;
            EQP_STAT = lstEIF[1].Trim();
            LOT_RUNNING = lstEIF[2].Trim().Equals("True") ? true : false;
            LOT_ID = lstEIF[3].Trim();
            DRY_RUN = lstEIF[4].Trim().Equals("True") ? true : false;
            IT_PASS = lstEIF[5].Trim().Equals("True") ? true : false;
            BIZIP = lstEIF[6].Trim();

            List<string> lstSYS = sysydata.Split(':').ToList();
            HOSTNAME = lstSYS[0];
            HOSTIP = lstSYS[1];
            CPU_USAGE = float.Parse(lstSYS[2]);
            RAM_TOTAL = Convert.ToUInt64(lstSYS[3]);
            RAM_USED = Convert.ToUInt64(lstSYS[4]);
            DISK_TOTAL = Convert.ToUInt64(lstSYS[5]);
            DISK_USED = Convert.ToUInt64(lstSYS[6]);

            ELMNAME = elmname;
            LOGIC = logic;

            #region PROC 정리
            if (LOGIC.ToUpper().Contains("MIXER"))
            {
                PROCID = 1;
                PROCNAME = "Mixer";
            }
            else if (LOGIC.ToUpper().Contains("CONVEYOR"))
            {
                PROCID = 2;
                PROCNAME = "Conveyor";
            }
            else if (LOGIC.ToUpper().Contains("INSCOATER"))
            {
                PROCID = 3;
                PROCNAME = "InsCoater";
            }
            else if (LOGIC.ToUpper().Contains("COATER"))
            {
                PROCID = 4;
                PROCNAME = "Coater";
            }
            else if (LOGIC.ToUpper().Contains("CALENDER"))
            {
                PROCID = 5;
                PROCNAME = "Multi Calender";
            }
            else if (LOGIC.ToUpper().Contains("ROLLPRESS"))
            {
                PROCID = 6;
                PROCNAME = "Roll Press";
            }
            else if (LOGIC.ToUpper().Contains("SLITTER"))
            {
                PROCID = 7;
                PROCNAME = "Slitter";
            }
            else if (LOGIC.ToUpper().Contains("NOTCHING"))
            {
                PROCID = 11;
                PROCNAME = "Notching";
            }
            else if (LOGIC.ToUpper().Contains("LOADER"))
            {
                PROCID = 12;
                PROCNAME = "Loader";
            }
            else if (LOGIC.ToUpper().Contains("LAMINATION"))
            {
                PROCID = 13;
                PROCNAME = "Lamination";
            }
            else if (LOGIC.ToUpper().Contains("STACKING"))
            {
                PROCID = 14;
                PROCNAME = "Stacking";
            }
            else if (LOGIC.ToUpper().Contains("AZS"))
            {
                PROCID = 15;
                PROCNAME = "AZS";
            }
            else if (LOGIC.ToUpper().Contains("PACKAGING"))
            {
                PROCID = 16;
                PROCNAME = "Packaging";
            }
            #endregion

            lstIp = lstIP;
        }

        public bool isDevIP()
        {
            foreach (string ip in lstIp) 
            {
                if (BIZIP.Equals(ip)) return false;
            }
            return true;
        }

        public bool isError()
        {
            TimeSpan t = DateTime.Now - DTTM;

            return t.TotalMinutes > 10 ? true : false;
        }
    }
}
