/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
                                                            ***** 03.07.2021 *****
 Написать функцию, приимающую на вход путь к директории с файлами .Net сборок (с расширением .dll). Функция должна возвращать имена всех public
и protected методов, которые содеражтся в классах, находящихся в этих .Net сборках, сгруппированные по именам классов.
Пример:
Входные значения: "C:\DEV\Assemblies"
Выходные значения: Class1: Execute, Check, Dispose; Class2: Execute, Dispose; Class3: Check.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace WpfApplication
{
    public partial class MainWindow : Window
    {
        static uint MaxNumButton = 65535; // максимальное количество директорий
        Brush brush = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FFFFFFFF");
        Brush bord = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#FFFFFFFF");
        Button[] ButtonsInWindow = new Button[MaxNumButton]; // создаем массив кнопок

        public MainWindow()
        {
            InitializeComponent();

            // обработчик нажатия на кнопку-поиск методов
            BtnSearch.Click += BtnSearchClick;

            // создаем кнопки локальных дисков
            int count = LocalDisks();

        }

        // создает новую кнопку с входными параметрами и добавляет ее в RightWindow
        int OutButton(int n, Brush bord, Brush brush, string s, System.Windows.RoutedEventHandler Btn)
        {
            ButtonsInWindow[n] = new Button() { HorizontalContentAlignment = 0, BorderBrush = bord, Background = brush, Visibility = 0, Content = s, Width = 371, Height = 20, FontFamily = new FontFamily("Times new Roman"), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Left };
            ButtonsInWindow[n].Content = s;
            WindowSection.Items.Add(ButtonsInWindow[n]);

            ButtonsInWindow[n].Click += Btn;
            n++;
            return n;
        }

        // создает кнопки ЛОКАЛЬНЫХ ДИСКОВ
        int LocalDisks()
        {
            string[] Drives = Environment.GetLogicalDrives();
            for (int i = 0; i < MaxNumButton; i++)
                WindowSection.Items.Remove(ButtonsInWindow[i]);
            int num = 1;
            // для каждой папки в директории создаем кнопку
            foreach (string s in Drives)
                num = OutButton(num, bord, brush, s, BtnClick);

            return num;
        }

        // создает кнопки ПАПОК
        int Directories(string Path)
        {
            // удаляем все кнопки из правого окна для дальнейшего отрисовывания новых разделов
            for (int i = 0; i < MaxNumButton; i++)
                WindowSection.Items.Remove(ButtonsInWindow[i]);

            string[] Drives = Directory.GetDirectories(Path); // получаем имена всех ПАПОК в директории

            // создаем первую кнопку -- выход в предыдщую директорию
            int num = OutButton(0, bord, brush, "[...]", BtnClickReturn);

            // для каждой папки в директории создаем кнопку
            foreach (string s in Drives)
                num = OutButton(num, bord, brush, s, BtnClick);

            return num;
        }
        
        // создает кнопки ФАЙЛОВ
        int Filess(int n, string path)
        {
            string[] Files = Directory.GetFiles(path); // получаем имена всех ФАЙЛОВ в директории
            foreach (string s in Files)
            {
                FileInfo fileInf = new FileInfo(s);
                //если CheckBox активирован И текущий рассматриваемый файл с расширением .dll ИЛИ CheckBox неактивирован, то создаем кнопку
                if ((bool)CheckName.IsChecked && fileInf.Extension != ".dll" || !(bool)CheckName.IsChecked)
                    n = OutButton(n, bord, brush, s, BtnClick);
            }

            return n;
        }
        
        // обработчик нажатия на кнопку-директорию
        void BtnClick(object sender, RoutedEventArgs e)
        {
            Button senderBtn = sender as Button; // получаем текущий путь, как имя кнопки
            direct.Text = senderBtn.Content.ToString(); // записываем его в окно ввода пути

            // если в текущем пути НЕ файл, то открываем эту директорию
            if (Directory.Exists(senderBtn.Content.ToString()))
            {
                // создаем кнопки всех папок в текущей директории
                int num = Directories(direct.Text);

                // создаем кнопки всех файлов в текущей директории
                //num = Filess(num, direct.Text);
            }
        }

        // обработчик нажатия на кнопку возврата в предыдущую директорию
        void BtnClickReturn(object sender, RoutedEventArgs e)
        {
            int num = 0;
            if (new DirectoryInfo(direct.Text).FullName == new DirectoryInfo(direct.Text).Root.FullName)
                num = LocalDisks();
            else
            {
                direct.Text = System.IO.Directory.GetParent(direct.Text).FullName; // устанавливаем родителя текущей папки текущей папкой

                // создаем кнопки всех папок в текущей директории
                num = Directories(direct.Text);

                // создаем кнопки всех файлов в текущей директории
                num = Filess(num, direct.Text);
            }
        }

        void BtnSearchClick(object sender, RoutedEventArgs e)
        {
            if (direct.Text == "")
            {
                OutBox.Text += Environment.NewLine + "Empty path!";
                MessageBox.Show("Empty path!");
            }
            else
            {
                List<string> library = new List<string>();
                library.AddRange(Directory.EnumerateFiles(direct.Text, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".dll")));
                foreach (var lib in library)
                {
                    Assembly assembly = typeof(MainWindow).Assembly;
                    foreach (var type in assembly.GetTypes())
                    {
                        OutBox.Text += Environment.NewLine + type.Name;
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic))
                        {
                            if (method.IsFamily || method.IsPublic)
                            {
                                OutBox.Text += Environment.NewLine + "-" + method.Name;
                            }
                        }
                    }
                }
            }
            MessageBox.Show("Click proccesing is complete!");
        }
    }
}