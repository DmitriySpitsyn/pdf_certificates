using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertificateGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PdfReader PDFReader = new PdfReader(Environment.CurrentDirectory + @"\Пример.pdf");
            string filenamesave = Environment.CurrentDirectory + @"\"+
                    DateTime.UtcNow.Hour.ToString()
                    + DateTime.UtcNow.Minute.ToString()
                    + DateTime.UtcNow.Second.ToString() + ".pdf";
                FileStream Stream = new FileStream(filenamesave, FileMode.Create, FileAccess.Write);

            PdfStamper PDFStamper = new PdfStamper(PDFReader, Stream);
            AcroFields acrf = PDFStamper.AcroFields;
            acrf.SetField("Реквизиты", richTextBox1.Text);
            acrf.SetField("НомерСертификата", DateTime.Now.Millisecond.ToString());
            acrf.SetField("ДатаКалибровки", dateTimeCalibration.Value.ToShortDateString());
            acrf.SetField("ОбьектКалибровки", ObjectCalibration.Text);



            PDFStamper.Close();
            PDFReader.Close();
            Stream.Close();
            System.Diagnostics.Process.Start(filenamesave);
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=" + Environment.CurrentDirectory + @"\" + "DbCore.db" + "; Version=3;"))
            {
                Connect.Open();
                SQLiteCommand comm = new SQLiteCommand("Insert INto Certificates Values('" + 
                    DateTime.Now.Millisecond.ToString()+"','"+
                    ObjectCalibration.Text + "'," +
                    "CURRENT_TIMESTAMP"+
                    ");", Connect);
                comm.ExecuteNonQuery();
                Connect.Close();

            }

            }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=" + Environment.CurrentDirectory + @"\" + "DbCore.db" + "; Version=3;"))
            {
                Connect.Open();
                SQLiteCommand comm = new SQLiteCommand("Select * From Certificates", Connect);
                using (SQLiteDataReader read = comm.ExecuteReader())
                {
                    while (read.Read())
                    {
                        dataGridView1.Rows.Add(new object[] {
            //read.GetValue(0),  // U can use column index
            read.GetValue(read.GetOrdinal("Number")),  // Or column name like this
            read.GetValue(read.GetOrdinal("Object")),
            read.GetValue(read.GetOrdinal("Date"))
            });
                    }
                }
            }
        }
    }
}
