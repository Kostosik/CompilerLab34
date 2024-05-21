using Compiler.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Compiler.CustomTabPage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Compiler
{
    public partial class MainForm : Form
    {
        Stream fileStream;
        public string filePath;
        bool isEdited = false;
        Lexer lexer;
        Parser parser;
        #region Атрибуты формы
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        inputTabControl inputTab;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            this.Text = AssemblyTitle;

            this.KeyPreview = true;

            this.InputLanguageChanged += (sender, e) =>
            {
                languageKeyLabel.Text = string.Format("Язык ввода: {0}", InputLanguage.CurrentInputLanguage.LayoutName);
            };
            CapsLockLabel.Text = string.Format("Клавиша CapsLock: " + (Control.IsKeyLocked(Keys.CapsLock) ? "Нажата" : "Не нажата"));
            languageKeyLabel.Text = string.Format("Язык ввода: {0}", InputLanguage.CurrentInputLanguage.LayoutName);

            inputTab = new inputTabControl();
            inputTab.Parent = this;
            mainSplitContainer.Panel1.Controls.Add(inputTab);
            inputTab.BringToFront();
            inputTab.GetTab().SelectedIndex = 0;
            (inputTab.GetTab().TabPages[0] as CustomTabPage).inputRichTextBox.TextChanged += new System.EventHandler(textChanged);
            inputTab.Size = new Size(mainSplitContainer.Width, mainSplitContainer.SplitterDistance);
            this.MainForm_Resize(null, null);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            mainSplitContainer.Size = new Size(this.Size.Width - 40, this.Height - 130);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();

            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                if ((tabControl.TabPages[i] as CustomTabPage).isFileSaved)
                    continue;
                else
                {
                    tabControl.SelectedIndex = i;
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.language == "english")
            {
                this.languageToolStripSplitButton.Image = Resources.флаганглии;
                ChangeLanguage("en");
            }
            if (Properties.Settings.Default.language == "russian")
            {
                this.languageToolStripSplitButton.Image = Resources.флагрф;
                ChangeLanguage("ru-RU");
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            CapsLockLabel.Text = string.Format("Клавиша CapsLock: " + (Control.IsKeyLocked(Keys.CapsLock) ? "Нажата" : "Не нажата"));
        }


        #region ToolStrip Buttons
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            var InfoForm = new InfoForm();
            InfoForm.Show();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (inputTab.tabControl1.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Copy();
        }

        private void CutButton_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (inputTab.tabControl1.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Cut();
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (inputTab.tabControl1.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Paste();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (inputTab.tabControl1.TabPages[inputTab.tabControl1.SelectedIndex] as CustomTabPage).Undo();
        }

        private void RepeatButton_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Redo();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            outputRichTextBox.Text = fileContent;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (filePath != null)
                File.WriteAllText(filePath, richTextBox1.Text);
            else
                сохранитьКакToolStripMenuItem_Click(sender, e);
        }

        private void RunButton_Click(object sender, EventArgs e)
        {

        }

        private void HelpButton1_Click(object sender, EventArgs e)
        {
            var HelpForm = new HelpForm();
            HelpForm.Show();
        }

        #endregion

        #region MenuStrip Buttons
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileStream != null)
            {
                fileStream.Close();
            }

            outputRichTextBox.Text = string.Empty;
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }
            outputRichTextBox.Text = fileContent;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filePath != null)
                File.WriteAllText(filePath, outputRichTextBox.Text);
            else
                сохранитьКакToolStripMenuItem_Click(sender, e);
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = "c:\\";
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = saveFileDialog.FileName;
                    var fileStream = saveFileDialog.OpenFile();

                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(outputRichTextBox.Text);
                    }
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveForm saveForm = new SaveForm(this);
            if (isEdited)
                Close();
            else
                saveForm.Show();
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Undo();
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Redo();

        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Cut();
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Copy();
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.Paste();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.SelectedText = "";
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl tabControl = inputTab.GetTab();
            (tabControl.TabPages[tabControl.SelectedIndex] as CustomTabPage).inputRichTextBox.SelectAll();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var InfoForm = new InfoForm();
            InfoForm.Show();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var HelpForm = new HelpForm();
            HelpForm.Show();
        }
        #endregion

        #region Resize functions
        private void resizeFunction()
        {
            if (inputTab != null)
            {
                inputTab.Size = new Size(mainSplitContainer.Width, mainSplitContainer.SplitterDistance);
            }
            outputRichTextBox.Size = new Size(mainSplitContainer.Width - 10, mainSplitContainer.Size.Height - mainSplitContainer.Panel1.Size.Height - 10 - statusStrip1.Height);
        }

        private void mainSplitContainer_Resize(object sender, EventArgs e)
        {
            resizeFunction();
        }

        private void mainSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            resizeFunction();
        }
        #endregion

        #region Смена языка

        private void ChangeLanguage(string lang)
        {
            foreach (Control c in this.Controls)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
                resources.ApplyResources(c, c.Name, new CultureInfo(lang));
            }
            foreach (ToolStripMenuItem item in this.MainMenuStrip.Items)
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
                resources.ApplyResources(item, item.Name, new CultureInfo(lang));
            }
            foreach (object item in this.toolStrip.Items)
            {
                if (!(item is ToolStripButton))
                    continue;
                ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
                resources.ApplyResources(item, (item as ToolStripButton).Name, new CultureInfo(lang));
            }
        }

        private void русскийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageToolStripSplitButton.Image = Resources.флагрф;
            ChangeLanguage("ru-RU");
            Properties.Settings.Default.language = "russian";
            Properties.Settings.Default.Save();
        }

        private void английскийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            languageToolStripSplitButton.Image = Resources.флаганглии;
            ChangeLanguage("en");
            Properties.Settings.Default.language = "english";
            Properties.Settings.Default.Save();
        }
        #endregion

        public void textChanged(object sender, EventArgs e)
        {
            lexer = new Lexer();
            parser = new Parser();
            parser.state = State.STATE_INIT;
            ImprovedRichTextBox localTempTextBox = (sender as ImprovedRichTextBox);
            (localTempTextBox.Parent as CustomTabPage).isFileSaved = false;

            string expr = localTempTextBox.Text;
            List<Token> tokens = lexer.tokenize(expr);
            int error_count = 0;

            outputRichTextBox.Text = "";

            bool foundErrorInLine = false;

            for (int i = 0; i < tokens.Count; i++)
            {
                ParseResult error = parser.parse(tokens[i]);
                if (error.is_error)
                {
                    if (!foundErrorInLine)
                    {
                        outputRichTextBox.AppendText(error.Stringize(expr) + "\n");
                        error_count++;
                        foundErrorInLine = true;
                    }
                }
                else
                {
                    foundErrorInLine = false;
                }
            }

            outputRichTextBox.AppendText("\nВсего ошибок: " + error_count + "\n");

            parser = new Parser();
            parser.state = State.STATE_INIT;
            richTextBox1.Text = "";
            foreach (var token in tokens)
            {
                ParseResult error = parser.parse(token);
                if (!error.is_error)
                    richTextBox1.Text += error.actualValue() + " ";
            }
        }
    }
}