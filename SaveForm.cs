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

namespace Compiler
{
    public partial class SaveForm : Form
    {
        MainForm mainForm;
        string filePath1;

        public SaveForm(MainForm owner)
        {
            InitializeComponent();
            mainForm = owner;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            filePath1 = mainForm.filePath;

            if (filePath1 != null)
                File.WriteAllText(filePath1, mainForm.outputRichTextBox.Text);
            else
            {
                var filePath1 = string.Empty;

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.InitialDirectory = "c:\\";
                    saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                    saveFileDialog.FilterIndex = 2;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath1 = saveFileDialog.FileName;
                        var fileStream = saveFileDialog.OpenFile();

                        using (StreamWriter writer = new StreamWriter(fileStream))
                        {
                            writer.Write(mainForm.outputRichTextBox.Text);
                        }
                    }
                }
            }
            this.Close();
            mainForm.Close();
        }

        private void buttonNotSave_Click(object sender, EventArgs e)
        {
            this.Close();
            mainForm.Close();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveForm_Load(object sender, EventArgs e)
        {

        }
    }
}
