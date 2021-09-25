using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace G2Converter
{
    public partial class G2Converter : Form
    {
        public G2Converter()
        {
            InitializeComponent();
        }

        //Drag Enter
        void ListBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        //Drag Drop
        void ListBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                //Declare File Type
                if (!file.ToString().Contains("xml") && !file.ToString().Contains("html"))
                {
                    MessageBox.Show("Lütfen Geçerli bir dosya yükleyiniz.");
                }
                else
                {
                    listBox1.Items.Add(file.ToString());
                }
            }
            ConvertBtn.Enabled = true;
        }

        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                Convert();
            }
            else
            {
                MessageBox.Show("Lütfen sol taraftaki listeden çevrilecek olan dosyayı seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Convert Operation
        private void Convert()
        {
            string fileName = listBox1.SelectedItem.ToString();
            XDocument xmlFile = XDocument.Load(fileName);
            //Result File
            Random rnd = new Random();
            var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"Sonuc{rnd.Next()}.pdf");

            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var doc = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(doc, fs))
                    {
                        doc.Open();

                        //Column count
                        var columnCount = xmlFile.Root.Elements().First().Nodes().Count();

                        //Create a table with one column for every child node of elements
                        var t = new PdfPTable(columnCount)
                        {
                            //Flag that the first row should be repeated on each page break
                            HeaderRows = 1
                        };

                        //Loop through the first item to output column headers
                        foreach (var N in xmlFile.Root.Elements().First().Elements())
                        {
                            t.AddCell(N.Name.ToString());
                        }

                        //Loop through each CD row (this is so we can call complete later on)
                        foreach (var CD in xmlFile.Root.Elements())
                        {
                            //Loop through each child of the current CD. Limit the number of children to our initial count just in case there are extra nodes.
                            foreach (var N in CD.Elements().Take(columnCount))
                            {
                                t.AddCell(N.Value);
                            }
                            //Just in case any rows have too few cells fill in any blanks
                            t.CompleteRow();
                        }
                        //Add the table to the document
                        doc.Add(t);
                        doc.Close();
                        MessageBox.Show("Döküman Başarıyla Çevrildi", "Çevirme Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void G2Converter_Load(object sender, EventArgs e)
        {
            //Disable Convert Button
            if (listBox1.Items.Count == 0)
            {
                ConvertBtn.Enabled = false;
            }
        }
    }
}
