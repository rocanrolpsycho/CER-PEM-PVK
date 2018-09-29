using Chilkat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

namespace PEM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Path_key = "";
        public string Path_pem = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //BUSCA ARCHIVO
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            string filename = "";

            //PONE LOS MYME TYPES
            dlg.DefaultExt = ".cer";
            dlg.Filter = "CERT Files (*.cer)|*.cer";

            //GUARDA RESULTADO
            Nullable<bool> result = dlg.ShowDialog();


            //REVISA QUE NO SEA NULO
            if (result != true)
            {
                return;
            }

            //ABRE EL DOCUMENTO
            filename = dlg.FileName;
            Cert cert = new Cert();
            //CARGA EL ARCHIVO ALV
            bool success = cert.LoadFromFile(filename);
            if (success != true)
            {
                Debug.WriteLine(cert.LastErrorText);
                MessageBox.Show("Error");
                return;
            }

            //GIARDA EL ARCHIVO .PEM DEL CER
            success = cert.ExportCertPemFile("GenerateCer.pem");
            if (success != true)
            {
                Debug.WriteLine(cert.LastErrorText);
                MessageBox.Show("Error");
                return;
            }

            MessageBox.Show ("Success");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!Valida())
            {
                MessageBox.Show("Falta Informacion");
            }

            P12();
            return;

        }

        public bool Valida()
        {
            if (this.Path_key == "") { return false; }
            if (this.Path_pem == "") { return false; }
            if (this.txt_Pass.Text == "") { return false; }
            return true;
        }

        public void P12()
        {
            Chilkat.Cert cert = new Chilkat.Cert();

            bool success = cert.LoadFromFile(this.Path_pem);
            if (success != true)
            {
                MessageBox.Show("Error");
                return;
            }

            Chilkat.CertChain certChain = cert.GetCertChain();
            if (cert.LastMethodSuccess != true)
            {
                MessageBox.Show("Error");
                return;
            }

            Chilkat.PrivateKey privKey = new Chilkat.PrivateKey();
            success = privKey.LoadPvkFile(this.Path_key, this.txt_Pass.Text);
            if (success != true)
            {
                MessageBox.Show("Error");
                return;
            }

            Chilkat.Pfx pfx = new Chilkat.Pfx();
            success = pfx.AddPrivateKey(privKey, certChain);
            if (success != true)
            {
                //Debug.WriteLine(pfx.LastErrorText);
                MessageBox.Show("Error");
                return;
            }

            success = pfx.ToFile(this.txt_Pass.Text, "GenerateP12.p12");
            if (success != true)
            {
                MessageBox.Show("Error");
                return;
            }

            MessageBox.Show("Success");
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            Chilkat.Cert cert = new Chilkat.Cert();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".key";
            dlg.Filter = "PEM Files (*.key)|*.key";
            Nullable<bool> result = dlg.ShowDialog();
            if (result != true)
            {
                return;
            }
            this.Path_key = dlg.FileName;
        }

        private void PEM_Click(object sender, RoutedEventArgs e)
        {
            Chilkat.Cert cert = new Chilkat.Cert();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".cer";
            dlg.Filter = "PEM Files (*.pem)|*.pem";
            Nullable<bool> result = dlg.ShowDialog();
            if (result != true)
            {
                return;
            }
            this.Path_pem = dlg.FileName;
        }
    }
}
