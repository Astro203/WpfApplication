/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
                                                            ***** 03.07.2021 *****
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using System.IO.Directory;

namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        static uint MaxNumButton = 65535; // максимальное количество директорий
        string[] Drives = Environment.GetLogicalDrives(); // получаем имена логических разделов
        Button[] ButtonsInLeftWindow = new Button[MaxNumButton]; // создаем массив кнопок
        string CurrentDirectory;// текущая директория

        public MainWindow()
        {
            InitializeComponent();

            int count = 0;

            var converter = new System.Windows.Media.BrushConverter(); // цвет для заливки кнопок
            var brush = (Brush)converter.ConvertFromString("#FFFFFFFF");
            converter = new System.Windows.Media.BrushConverter(); // цвет для границы кнопок
            var bord = (Brush)converter.ConvertFromString("#FFFFFFFF");

            // создаем кнопки в левом окне для каждого раздела
            foreach (string s in Drives)
            {
                 count = OutButton(count, bord, brush, s);
            }

        }

        Button[] ButtonsInRightWindow = new Button[MaxNumButton]; // создаем массив кнопок

        int OutButton(int n, Brush bord, Brush brush, string s)
        {
            ButtonsInRightWindow[n] = new Button() { HorizontalContentAlignment = 0, BorderBrush = bord, Background = brush, Visibility = 0, Content = s, Width = 371, Height = 20, FontFamily = new FontFamily("Times new Roman"), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Left };
            ButtonsInRightWindow[n].Content = s;
            RightWindowSection.Items.Add(ButtonsInRightWindow[n]);

            ButtonsInRightWindow[n].Click += BtnClick;
            n++;
            return n;
        }

        // обработчик нажатия на кнопку
        void BtnClick(object sender, RoutedEventArgs e)
        {
            var senderBtn = sender as Button; // получаем текущий путь, как имя кнопки
            direct.Text = senderBtn.Content.ToString();
            CurrentDirectory = senderBtn.Content.ToString();

            // если в текущем пути НЕ файл, то...
            if (Directory.Exists(senderBtn.Content.ToString()))
            {
                // удаляем все кнопки из правого окна для дальнейшего отрисовывания новых разделов
                for (int i = 0; i < MaxNumButton; i++)
                    RightWindowSection.Items.Remove(ButtonsInRightWindow[i]);

                string[] Drives = Directory.GetDirectories(senderBtn.Content.ToString()); // получаем имена всех папок в директории
                var converter = new System.Windows.Media.BrushConverter(); // цвет для фона кнопок
                var brush = (Brush)converter.ConvertFromString("#FFFFFFFF");

                converter = new System.Windows.Media.BrushConverter(); // цвет для границы кнопок
                var bord = (Brush)converter.ConvertFromString("#FFFFFFFF");

                // создаем первую кнопку -- выход в предыдщую директорию
                //DirectoryInfo directoryInfo = System.IO.Directory.GetParent(Drives[0]);
                ButtonsInRightWindow[0] = new Button() { HorizontalContentAlignment = 0, BorderBrush = bord, Background = brush, Visibility = 0, Width = 371, FontFamily = new FontFamily("Times new Roman"), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Left };
                ButtonsInRightWindow[0].Content = "[...]";
                RightWindowSection.Items.Add(ButtonsInRightWindow[0]);
                ButtonsInRightWindow[0].Click += BtnClickReturn;

                int num = 1;
                // для каждой папки в директории создаем кнопку
                foreach (string s in Drives)
                {
                    num = OutButton(num, bord, brush, s);
                }

                string[] Files = Directory.GetFiles(senderBtn.Content.ToString()); // получаем имена всех файлов в директории
                foreach (string s in Files)
                {
                    FileInfo fileInf = new FileInfo(s);
                    if ((bool)CheckName.IsChecked && fileInf.Extension == ".dll" || !(bool)CheckName.IsChecked)
                    {
                        num = OutButton(num, bord, brush, s);
                    }
                }
            }
        }

        // обработка кнопки возврата в предыдущую директорию
        void BtnClickReturn(object sender, RoutedEventArgs e)
        {
            var converter = new System.Windows.Media.BrushConverter(); // цвет для фона кнопок
            Brush brush = (Brush)converter.ConvertFromString("#FFFFFFFF");

            converter = new System.Windows.Media.BrushConverter(); // цвет для границы кнопок
            Brush bord = (Brush)converter.ConvertFromString("#FFFFFFFF");

            if (new DirectoryInfo(direct.Text).FullName == new DirectoryInfo(direct.Text).Root.FullName)
            {
                string[] Drives = Environment.GetLogicalDrives();
                for (int i = 0; i < MaxNumButton; i++)
                    RightWindowSection.Items.Remove(ButtonsInRightWindow[i]);
                int num = 1;
                // для каждой папки в директории создаем кнопку
                foreach (string s in Drives)
                {
                    num = OutButton(num, bord, brush, s);
                }
            }
            else
            {
                DirectoryInfo retPath = System.IO.Directory.GetParent(direct.Text);
                //MessageBox.Show(retPath.FullName);
                direct.Text = retPath.FullName;

            

            for (int i = 0; i < MaxNumButton; i++)
                RightWindowSection.Items.Remove(ButtonsInRightWindow[i]);

            string[] Drives = Directory.GetDirectories(direct.Text); // получаем имена всех папок в директории
            

            // создаем первую кнопку -- выход в предыдщую директорию
            //DirectoryInfo directoryInfo = System.IO.Directory.GetParent(Drives[0]);
            ButtonsInRightWindow[0] = new Button() { HorizontalContentAlignment = 0, BorderBrush = bord, Background = brush, Visibility = 0, Width = 371, FontFamily = new FontFamily("Times new Roman"), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Left };
            ButtonsInRightWindow[0].Content = "[...]";
            RightWindowSection.Items.Add(ButtonsInRightWindow[0]);
            ButtonsInRightWindow[0].Click += BtnClickReturn;

            int num = 1;
            // для каждой папки в директории создаем кнопку
            foreach (string s in Drives)
            {
                num = OutButton(num, bord, brush, s);
            }

            
            string[] Files = Directory.GetFiles(direct.Text); // получаем имена всех файлов в директории
            foreach (string s in Files)
            {
                FileInfo fileInf = new FileInfo(s);
                if ((bool)CheckName.IsChecked && fileInf.Extension == ".dll" || !(bool)CheckName.IsChecked)
                {
                    num = OutButton(num, bord, brush, s);
                }

                
            }
        }
        }

        

        
    }
}

/* string path = System.IO.Directory.GetCurrentDirectory();
            string filter = "*.exe";*/