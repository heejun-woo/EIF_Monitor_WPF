using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace EIF_Monitor_WPF
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        List<EIF_INFO> lstSYS = new List<EIF_INFO>();
        List<EIF_INFO> lstEIF = new List<EIF_INFO>();

        List<string> lstIP = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            loadXml();

            SelectData();
            ViewData();

        }

        private void loadXml()
        {


            XmlDocument xml = new XmlDocument();
            xml.Load("config.xml");

            XmlNodeList lst = xml.GetElementsByTagName("connectionStrings");

            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (XmlNode item in lst)
            {
                cbDB.Items.Add(new { Display = item["name"].InnerText, Value = item["string"].InnerText });
            }

            if (cbDB.Items.Count > 1) cbDB.SelectedIndex = 0;

            lst = xml.GetElementsByTagName("bizIP");

            foreach (XmlNode item in lst)
            {
                string[] str = item["string"].InnerText.Split(',');
                foreach (string s in str) lstIP.Add(s);
            }

        }


        private void SelectData()
        {

            lstEIF = new List<EIF_INFO>();
            for (int i = 0; i < cbDB.Items.Count; i++)
            {
                if (cbDB.Items[i].ToString().Contains("ORACLE"))
                {
                    OracleConnection conn = new OracleConnection((cbDB.Items[i] as dynamic).Value);
                    conn.Open();

                    string sql = " SELECT * FROM ";
                    sql += "       (SELECT ELMNO, UPDTTM, EIFDATA, SYSDATA ";
                    sql += " FROM(SELECT ELMNO, VARNAME, VARVALUE FROM ezControl_VAR_INTERNAL_VAL  result where VARNAME like 'V_EIF_MONITORING%') ";
                    sql += "  PIVOT(max(TO_CHAR(VARVALUE)) FOR VARNAME IN('V_EIF_MONITORING:V_UPDTTM' AS UPDTTM, 'V_EIF_MONITORING:V_EIF_DATA_01' AS EIFDATA, 'V_EIF_MONITORING:V_SYSTEM_DATA' AS SYSDATA)) ) A,   ";
                    sql += "  (SELECT ELMNO, (SELECT ELMNAME FROM ezControl_ELM A WHERE A.ELMNO = E.ELMNO_PR) || '(' || ELMNAME || ')' AS ELMNAME, L.LOGICCLS FROM ezControl_ELM E, ezControl_LOGIC L ";
                    sql += " WHERE E.LOGICCATNO = L.LOGICCATNO AND E.LOGICNO = L.LOGICNO ) B ";
                    sql += " WHERE A.ELMNO = B.ELMNO ";
                    OracleCommand cmd = new OracleCommand(sql, conn);
                    OracleDataReader mdr = cmd.ExecuteReader();

                    while (mdr.Read())
                    {
                        lstEIF.Add(new EIF_INFO(Convert.ToString(mdr[0]), Convert.ToString(mdr["UPDTTM"]), Convert.ToString(mdr["EIFDATA"]), Convert.ToString(mdr["SYSDATA"]),
                            Convert.ToString(mdr["ELMNAME"]), Convert.ToString(mdr["LOGICCLS"]), lstIP));
                    }
                    mdr.Close();
                    conn.Close();
                }
                else
                {
                    SqlConnection conn = new SqlConnection((cbDB.Items[i] as dynamic).Value);
                    conn.Open();

                    string sql = "SELECT * FROM ";
                    sql += " ( SELECT ELMNO, [V_EIF_MONITORING:V_UPDTTM] AS UPDTTM, [V_EIF_MONITORING:V_EIF_DATA_01] AS EIFDATA, [V_EIF_MONITORING:V_SYSTEM_DATA] AS SYSDATA ";
                    sql += "  FROM ( SELECT ELMNO, VARNAME, VARVALUE FROM ezControl_VAR_INTERNAL_VAL where VARNAME like 'V_EIF_MONITORING%') AS result ";
                    sql += "  PIVOT ( max(VARVALUE) FOR VARNAME IN ( [V_EIF_MONITORING:V_UPDTTM], [V_EIF_MONITORING:V_EIF_DATA_01], [V_EIF_MONITORING:V_SYSTEM_DATA]) ) AS pivot_resul ) AS A,  ";
                    sql += "  ( SELECT ELMNO, (SELECT ELMNAME  FROM ezControl_ELM A WHERE A.ELMNO = E.ELMNO_PR) +'(' + ELMNAME + ')' AS ELMNAME, L.LOGICCLS FROM ezControl_ELM E, ezControl_LOGIC L ";
                    sql += " WHERE E.LOGICCATNO = L.LOGICCATNO AND E.LOGICNO = L.LOGICNO ) AS B ";
                    sql += " WHERE A.ELMNO = B.ELMNO ";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader mdr = cmd.ExecuteReader();

                    while (mdr.Read())
                    {
                        lstEIF.Add(new EIF_INFO(Convert.ToString(mdr[0]), Convert.ToString(mdr["UPDTTM"]), Convert.ToString(mdr["EIFDATA"]), Convert.ToString(mdr["SYSDATA"]),
                            Convert.ToString(mdr["ELMNAME"]), Convert.ToString(mdr["LOGICCLS"]), lstIP));
                    }
                    mdr.Close();
                    conn.Close();
                }
            }


            Dictionary<string, EIF_INFO> dicSys = new Dictionary<string, EIF_INFO>();

            foreach (EIF_INFO info in lstEIF.OrderBy(x => x.HOSTNAME))
            {
                if (dicSys.ContainsKey(info.HOSTNAME))
                {
                    if (dicSys[info.HOSTNAME].DTTM < info.DTTM) dicSys[info.HOSTNAME] = info;
                }
                else
                {
                    dicSys.Add(info.HOSTNAME, info);
                }
            }

            lstSYS = dicSys.Values.ToList();

            int idxServer = cbServer.SelectedIndex;
            cbServer.Items.Clear();
            foreach (var item in lstEIF.OrderBy(x => x.HOSTNAME).GroupBy(x => x.HOSTNAME))
            {
                cbServer.Items.Add(item.Key);
            }
            cbServer.SelectedIndex = idxServer;


            int idxProcess = cbProcess.SelectedIndex;
            cbProcess.Items.Clear();
            foreach (var item in lstEIF.OrderBy(x => x.PROCNAME).GroupBy(x => x.PROCNAME))
            {
                if (item.Key == null) continue;
                cbProcess.Items.Add(item.Key);
            }
            cbProcess.SelectedIndex = idxProcess;

        }

        private void ViewData()
        {
            pnServer.Children.Clear();
            pnEIFList.Children.Clear();

            int cnt = 0;

            foreach (EIF_INFO info in lstEIF.OrderBy(x => x.PROCID).ThenBy(x => x.ELMNAME))
            {
                if (cbServer.SelectedIndex > -1)
                {
                    if (!cbServer.Items[cbServer.SelectedIndex].Equals(info.HOSTNAME)) continue;
                }

                if (cbProcess.SelectedIndex > -1)
                {
                    if (!cbProcess.Items[cbProcess.SelectedIndex].Equals(info.PROCNAME)) continue;
                }

                if (chkLotRunning.IsChecked == true && !info.LOT_RUNNING) continue;
                if (chkDryRun.IsChecked == true && !info.DRY_RUN) continue;
                if (chkItPass.IsChecked == true && !info.IT_PASS) continue;
                if (chkDevIP.IsChecked == true && !info.isDevIP()) continue;
                if (chkConsole.IsChecked == true && !info.isError()) continue;

                eif_console eif = new eif_console(info);

                pnEIFList.Children.Add(eif);

                cnt++;
            }

            foreach (EIF_INFO info in lstSYS.OrderBy(x => x.PROCID).ThenBy(x => x))
            {
                cnt = 0;

                SYS_Console sys = new SYS_Console(info);

                pnServer.Children.Add(sys);

                cnt++;
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SelectData();
            ViewData();
        }

    }

}
