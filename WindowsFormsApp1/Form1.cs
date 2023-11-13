using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Имя файла";
            
           
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;

                FileInfo fileInfo = new FileInfo(openFileDialog1.FileName);

                DateTime creationTime = fileInfo.CreationTime;

                textBox2.Text = Convert.ToString(creationTime);
             }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pattern = @"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.\d{4} (0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$";
            string newDateTimeString = textBox3.Text;
            string filePath = textBox1.Text;
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(newDateTimeString))
            {
                MessageBox.Show("Дата указана не в верном формате, необходимо: (dd.mm.yyyy HH:MM)");
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (DateTime.TryParseExact(newDateTimeString, "dd.MM.yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime newDateTime))
                {

                    try
                    {
                        if (File.Exists(filePath))
                        {
                            // Закрываем все открытые ресурсы файла
                            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                            {
                                fs.Close();
                            }

                            // Получаем текущую информацию о файле
                            FileInfo fileInfo = new FileInfo(filePath);

                            // Создаем новый объект FileInfo с тем же путем
                            // и изменяем дату создания в этом объекте
                            FileInfo newFileInfo = new FileInfo(filePath)
                            {
                                CreationTime = newDateTime
                            };

                            // Заменяем существующий файл новым файлом с обновленной датой создания
                            //newFileInfo.Replace(filePath, null, true);

                            MessageBox.Show($"Дата создания изменена на: {newFileInfo.CreationTime}");
                        }
                        else
                        {
                            MessageBox.Show("Файл не существует.");
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                else
                {
                    MessageBox.Show("TRBL1 WITH PARSING");
                }
               

                

            }
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
