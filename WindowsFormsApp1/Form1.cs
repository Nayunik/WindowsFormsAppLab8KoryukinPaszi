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
using System.Security.Cryptography;
using System.Reflection;
using System.Security.AccessControl;
using System.IO.Pipes;
using System.Security.Principal;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public readonly int CountOpen = 3;
        public readonly int numberToEncryptFirstTime = 1;
        private const string CounterFilePath = "counter.txt";
        private const string Key = "0123456789ABCDEF0123456789ABCDEF";

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

                textBox5.Text = Convert.ToString(fileInfo.LastWriteTime);

                textBox7.Text = Convert.ToString(fileInfo.LastAccessTime);

                if ((fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                {
                    checkedListBox2.SetItemChecked(0, true);
                }
                else
                {
                    checkedListBox2.SetItemChecked(0, false);
                }

                if ((fileInfo.Attributes & FileAttributes.Normal) == FileAttributes.Normal)
                {
                    checkedListBox2.SetItemChecked(1, true);
                }
                else
                {
                    checkedListBox2.SetItemChecked(1, false);
                }

                if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    checkedListBox2.SetItemChecked(2, true);
                }
                else
                {
                    checkedListBox2.SetItemChecked(2, false);
                }

                try
                {
                    // Получаем информацию о правах доступа к файлу
                    FileSecurity fileSecurity = fileInfo.GetAccessControl();

                    // Получаем права текущего пользователя
                    AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

                    foreach (FileSystemAccessRule rule in rules)
                    {
                        // Сравниваем права доступа и устанавливаем галочки в CheckedListBox
                        if (rule.FileSystemRights.HasFlag(FileSystemRights.FullControl) && rule.IdentityReference.Equals(WindowsIdentity.GetCurrent().User))
                            checkedListBox1.SetItemChecked(0, true);
                        if (rule.FileSystemRights.HasFlag(FileSystemRights.Modify) && rule.IdentityReference.Equals(WindowsIdentity.GetCurrent().User))
                            checkedListBox1.SetItemChecked(1, true);
                        if (rule.FileSystemRights.HasFlag(FileSystemRights.ReadAndExecute) && rule.IdentityReference.Equals(WindowsIdentity.GetCurrent().User))
                            checkedListBox1.SetItemChecked(2, true);
                        if (rule.FileSystemRights.HasFlag(FileSystemRights.Read) && rule.IdentityReference.Equals(WindowsIdentity.GetCurrent().User))
                            checkedListBox1.SetItemChecked(3, true);
                        if (rule.FileSystemRights.HasFlag(FileSystemRights.Write) && rule.IdentityReference.Equals(WindowsIdentity.GetCurrent().User))
                            checkedListBox1.SetItemChecked(4, true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string pattern = @"^(0[1-9]|[12][0-9]|3[01])\.(0[1-9]|1[0-2])\.\d{4} (0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$";
            string newDateTimeString = textBox3.Text;
            string newDateTimeWriteString = textBox4.Text;
            string newDateTimeAccessString = textBox6.Text;
            string filePath = textBox1.Text;
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(newDateTimeString))
            {
                MessageBox.Show("Дата указана не в верном формате, необходимо: (dd.mm.yyyy HH:MM)");
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (DateTime.TryParseExact(newDateTimeString, "dd.MM.yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime newDateTime) && DateTime.TryParseExact(newDateTimeWriteString, "dd.MM.yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime newDateTimeWrite) && DateTime.TryParseExact(newDateTimeAccessString, "dd.MM.yyyy HH:mm:ss", null, DateTimeStyles.None, out DateTime newDateTimeAccess))
                {

                    try
                    {
                        if (File.Exists(filePath))
                        {
                            // Создаем новый объект FileInfo с тем же путем
                            // и изменяем даты в этом объекте
                            FileInfo newFileInfo = new FileInfo(filePath)
                            {
                                Attributes = 0,
                                CreationTime = newDateTime,
                                LastWriteTime = newDateTimeWrite,
                                LastAccessTime = newDateTimeAccess,
                            };

                            for (int i = 0; i < checkedListBox2.Items.Count; i++)
                            {

                                // Изменение атрибутов в соответствии с флажками в CheckedListBox
                                if (checkedListBox2.GetItemChecked(i) && checkedListBox2.Items[i].ToString() == "Hidden")
                                {
                                    // Установка атрибута Hidden
                                    newFileInfo.Attributes |= FileAttributes.Hidden;
                                }

                                if (checkedListBox2.GetItemChecked(i) && checkedListBox2.Items[i].ToString() == "Normal")
                                {
                                    // Установка атрибута Hidden
                                    newFileInfo.Attributes |= FileAttributes.Normal;
                                }

                                if (checkedListBox2.GetItemChecked(i) && checkedListBox2.Items[i].ToString() == "ReadOnly")
                                {
                                    // Установка атрибута Hidden
                                    newFileInfo.Attributes |= FileAttributes.ReadOnly;
                                }
                            }

                            // Очищаем все права на файл для текущего пользователя
                            FileSecurity fileSecurity = newFileInfo.GetAccessControl(); 
                            fileSecurity.PurgeAccessRules(new SecurityIdentifier(WindowsIdentity.GetCurrent().User.Value));
                            File.SetAccessControl(filePath, fileSecurity);

                            // Получаем выбранные права из CheckedListBox
                            FileSystemRights selectedRights = GetSelectedRights(checkedListBox1);

                            // Назначаем выбранные права на файл
                            SetFilePermissions(filePath, selectedRights);

                            
                            MessageBox.Show($"Свойства файла успешно изменены!");


                        }
                        else
                        {
                            MessageBox.Show("Файл не существует.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("TRBL1 WITH PARSING");
                }
               

                

            }
        }

        static FileSystemRights GetSelectedRights(CheckedListBox checkedListBox)
        {
            FileSystemRights selectedRights = 0;

            foreach (var item in checkedListBox.CheckedItems)
            {
                switch (item.ToString())
                {
                    case "Полный доступ":
                        selectedRights |= FileSystemRights.FullControl;
                        break;
                    case "Изменение":
                        selectedRights |= FileSystemRights.Modify;
                        break;
                    case "Чтение и выполнение":
                        selectedRights |= FileSystemRights.ReadAndExecute;
                        break;
                    case "Чтение":
                        selectedRights |= FileSystemRights.Read;
                        break;
                    case "Запись":
                        selectedRights |= FileSystemRights.Write;
                        break;
                }
            }

            return selectedRights;
        }

        static void SetFilePermissions(string filePath, FileSystemRights selectedRights)
        {
            FileSecurity fileSecurity = new FileSecurity(filePath, AccessControlSections.All);
            fileSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WindowsIdentity.GetCurrent().User.Value), selectedRights, AccessControlType.Allow));
            File.SetAccessControl(filePath, fileSecurity);
        }

        private FileSystemRights GetFileSystemRights(string accessLevel)
        {
            // Метод, который преобразует строку доступа в соответствующее значение FileSystemRights
            switch (accessLevel)
            {
                case "Полный доступ":
                    return FileSystemRights.FullControl;
                case "Изменение":
                    return FileSystemRights.Modify;
                case "Чтение и выполнение":
                    return FileSystemRights.ReadAndExecute;
                case "Чтение":
                    return FileSystemRights.Read;
                default:
                    throw new ArgumentException("Недопустимый уровень доступа");
            }
        }
        private void textBox1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (CheckLaunchLimit())
            {
               MessageBox.Show("Превышено количество разрешенных запусков.");
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Программа запускается.");
                IncrementLaunchCounter();

                try
                {
                    if (File.Exists(CounterFilePath))
                    {
                        // Закрываем все открытые ресурсы файла
                        using (FileStream fs = File.Open(CounterFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            fs.Close();
                        }

                        // Получаем текущую информацию о файле
                        FileInfo fileInfo = new FileInfo(CounterFilePath);

                        // Создаем новый объект FileInfo с тем же путем
                        // и изменяем дату создания в этом объекте
                        FileInfo newFileInfo = new FileInfo(CounterFilePath)
                        {
                            LastWriteTime = fileInfo.CreationTime,
                            LastAccessTime  = fileInfo.CreationTime
                        };
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        static bool CheckLaunchLimit()
        {
            int launchLimit = 99; // Установите нужное количество разрешенных запусков

            // Проверяем наличие файла счетчика
            if (!File.Exists(CounterFilePath))
            {
                // Если файла нет, создаем и записываем в него зашифрованный счетчик со значением 1
                string initialCounterValue = EncryptString("1", Key);
                File.Create(CounterFilePath).Close();
                File.WriteAllText(CounterFilePath, initialCounterValue);

                return false;
            }

            // Читаем зашифрованный счетчик из файла
            string encryptedCounter = File.ReadAllText(CounterFilePath);

            // Расшифровываем счетчик
            string decryptedCounter = DecryptString(encryptedCounter, Key);

            // Пытаемся преобразовать расшифрованный счетчик в число
            if (int.TryParse(decryptedCounter, out int launchCount))
            {
                // Если количество запусков превышено, возвращаем true
                return launchCount >= launchLimit;
            }

            // Если произошла ошибка при преобразовании, возвращаем false
            return false;
        }

        static void IncrementLaunchCounter()
        {
            // Читаем зашифрованный счетчик из файла
            string encryptedCounter = File.ReadAllText(CounterFilePath);

            // Расшифровываем счетчик
            string decryptedCounter = DecryptString(encryptedCounter, Key);

            // Пытаемся преобразовать расшифрованный счетчик в число
            if (int.TryParse(decryptedCounter, out int launchCount))
            {
                // Инкрементируем счетчик
                launchCount++;

                // Шифруем и сохраняем обновленный счетчик в файл
                string encryptedUpdatedCounter = EncryptString(launchCount.ToString(), Key);
                File.WriteAllText(CounterFilePath, encryptedUpdatedCounter);
            }
        }

        static string EncryptString(string input, string key)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        static string DecryptString(string cipherText, string key)
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private void ьсяПодменюСправкаСКомандойОПрограммеПриToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Автор:\r\nКорюкин Данил\r\nСтудент группы: ИТ-1035119\r\n", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
