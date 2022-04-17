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
using System.Windows.Shapes;

namespace FSTEKParser
{
    /// <summary>
    /// Interaction logic for SingleThreatInfo.xaml
    /// </summary>
    public partial class TextBoxWindow : Window
    {
        public TextBoxWindow()
        {
            InitializeComponent();
        }
        public TextBoxWindow(Threat threat)
        {
            InitializeComponent();
            Title = $"{threat.ThreatId}. {threat.ThreatName}";
            TextBoxWindowTextBox.Text = threat.ToString();
        }
        public TextBoxWindow(string str, string title)
        {
            InitializeComponent();
            Title = title;
            TextBoxWindowTextBox.Text = str;
        }
    }
}
