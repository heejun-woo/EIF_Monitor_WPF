using System;
using System.Collections.Generic;
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

namespace EIF_Monitor_WPF
{
    /// <summary>
    /// eif_console.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class eif_console : UserControl
    {
        public eif_console(EIF_INFO info)
        {
            InitializeComponent();

            lbEQP_Name.Header = info.ELMNAME;
            lbEQP_Name.ToolTip = info.HOSTIP + "[ " + info.HOSTNAME + " ] "; 

            if (!info.DRIVER_CONNECTED)
            {
                //lbEQP_Name.Header += " Discontinued";
                lbEQP_Name.Foreground = Brushes.Red;
            }

            //if (info.isError())
            //{
            //    lbEQP_Name.Foreground = Brushes.Pink;
            //    lbEQP_Name.Content = "Console Error";
            //    return;
            //}

            switch (info.EQP_STAT)
            {
                case "1":
                    lbStatus.Content = "R";
                    lbStatus.Foreground = Brushes.Green;
                    break;

                case "2":
                    lbStatus.Content = "W";
                    lbStatus.Foreground = Brushes.Yellow;
                    break;

                case "4":
                    lbStatus.Content = "T";
                    lbStatus.Foreground = Brushes.Red;
                    break;

                case "8":
                    lbStatus.Content = "U";
                    lbStatus.Foreground = Brushes.Blue;
                    break;

                default:
                    lbStatus.Content = "O";
                    lbStatus.Foreground = Brushes.Gray;
                    break;

            }
            if (info.LOT_RUNNING)
            {
                lbLotRunning.Content = info.LOT_ID;
            }
            else
            {
                X.Children.Remove(lbLotRunning);
            }
            if (!info.DRY_RUN)
            {
                X.Children.Remove(lbDryRun);
            }
            if (!info.IT_PASS)
            {
                X.Children.Remove(lbItPass);
            }
            if (!info.isDevIP())
            {
                X.Children.Remove(lbDEV);
            }
        }
    }
}
