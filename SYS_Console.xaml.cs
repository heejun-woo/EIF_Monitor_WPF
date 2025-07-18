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
    /// SYS_Console.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SYS_Console : UserControl
    {
        public SYS_Console(EIF_INFO info)
        {
            InitializeComponent();

            lbHostname.Content = info.HOSTNAME;
            lbIP.Content = info.HOSTIP;

            barCPU.Value = Convert.ToInt32(info.CPU_USAGE);
            if (barCPU.Value > 80) barCPU.Foreground = Brushes.Red;
            else if (barCPU.Value > 60) barCPU.Foreground = Brushes.Yellow;
            lbCpu.Content = info.CPU_USAGE.ToString("F1") + "%";

            float ramUsage = (float)info.RAM_USED / (float)info.RAM_TOTAL * 100;
            barRAM.Value = Convert.ToInt32(ramUsage);
            if (barRAM.Value > 80) barRAM.Foreground = Brushes.Red;
            else if (barRAM.Value > 60) barRAM.Foreground = Brushes.Yellow;
            lbRam.Content = ramUsage.ToString("F1") + "%";


            float diskUsage = (float)info.DISK_USED / (float)info.DISK_TOTAL * 100;
            barDISK.Value = Convert.ToInt32(diskUsage);
            if (barDISK.Value > 80) barDISK.Foreground = Brushes.Red;
            else if (barDISK.Value > 60) barDISK.Foreground = Brushes.Yellow;
            lbDisk.Content = diskUsage.ToString("F1") + "%";

        }
    }
}
