using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PCInfoParser_Client_NET_Service_Configurator
{
    public partial class Configurator : Form
    {
        Service service = new("PCInfoParcer");
        IniFile ini;
        public string[] user = new string[3];
        public string[] server = new string[3];
        public string app;
        public string genPath;
        public string logFile;
        public string idClient;
        public Configurator(string iniFile, string genPath, string logFile)
        {
            OldConfig oldconfig = new("C:\\Program Files\\ConfigNKU\\confignku.txt");
            ini = new(iniFile, oldconfig.GetValues());
            this.logFile = logFile;
            this.genPath = genPath;
            InitializeComponent();
        }


        private async void PrintLayers(int active = 0)
        {
            if (active != 3)
            {
                toolStripStatusLabel1.Text = service.GetStatus();
                active = service.GetStatusInCode();
            }

            await Task.Delay(100);
            switch (active)
            {
                case 0:
                    this.groupBox1.Enabled = false;
                    this.groupBox2.Enabled = false;
                    this.groupBox3.Enabled = false;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = false;
                    saveButton.Enabled = false;
                    break;
                case 1:
                    this.groupBox1.Enabled = true;
                    this.groupBox2.Enabled = true;
                    this.groupBox3.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = true;
                    saveButton.Enabled = true;
                    break;
                case 2:
                    this.groupBox1.Enabled = false;
                    this.groupBox2.Enabled = false;
                    this.groupBox3.Enabled = false;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    saveButton.Enabled = false;
                    break;
                case 3:
                    this.groupBox1.Enabled = false;
                    this.groupBox2.Enabled = false;
                    this.groupBox3.Enabled = false;
                    button1.Enabled = true;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    saveButton.Enabled = false;
                    break;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            // Обработчик события для кнопки сохранения настроек
            // В этом методе можно выполнить сохранение настроек в файл или базу данных
            // и закрыть форму
            this.DialogResult = DialogResult.OK;
            ini.SetValue("User", "ФИО", textBox1.Text);
            ini.SetValue("User", "Кабинет", textBox2.Text);
            ini.SetValue("User", "Организация", textBox3.Text);
            ini.SetValue("Server", "IP", textBox4.Text);
            ini.SetValue("Server", "Port", textBox5.Text);
            ini.SetValue("Server", "Password", textBox6.Text);
            ini.SetValue("App", "Autosend", textBox7.Text);
            ini.Save();
        }

        private void UserSetting_Load(object sender, EventArgs e)
        {
            textBox1.Text = ini.GetValue("User", "ФИО");
            textBox2.Text = ini.GetValue("User", "Кабинет");
            textBox3.Text = ini.GetValue("User", "Организация");
            textBox4.Text = ini.GetValue("Server", "IP");
            textBox5.Text = ini.GetValue("Server", "Port");
            textBox6.Text = ini.GetValue("Server", "Password");
            textBox7.Text = ini.GetValue("App", "Autosend");
            idClient = ini.GetValue("User", "ID");
            PrintLayers();
            if (idClient == "NeedToGet") this.label8.Text = "Ваш ID: Не получен";
            else this.label8.Text = $"Ваш ID: {idClient}";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                                                      (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем, нажата ли клавиша Ctrl+V (вставка из буфера обмена)
            if (e.Control && e.KeyCode == Keys.V)
            {
                // Получаем текст из буфера обмена
                string clipboardText = Clipboard.GetText();

                // Проверяем, содержит ли вставляемый текст символы, отличные от цифр
                if (!Regex.IsMatch(clipboardText, @"^\d+$"))
                {
                    // Если вставляемый текст содержит символы, отличные от цифр,
                    // то отменяем вставку
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Останавливается";
            PrintLayers(3);
            service.Stop();
            while (File.Exists(genPath))
            {
                await Task.Delay(500);
            }
            PrintLayers();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            SaveButton_Click(sender, e);
            toolStripStatusLabel1.Text = "Запускается";
            PrintLayers(3);
            service.Start();
            while (!File.Exists(genPath))
            {
                await Task.Delay(500);
            }
            PrintLayers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(logFile);
        }
    }
}
