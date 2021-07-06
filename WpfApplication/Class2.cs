using System;

namespace MyFunctions
{
    public class Util
    {
        int OutButton(int n, Brush bord, Brush brush, string s)
        {
            ButtonsInRightWindow[n] = new Button() { HorizontalContentAlignment = 0, BorderBrush = bord, Background = brush, Visibility = 0, Content = s, Width = 371, Height = 20, FontFamily = new FontFamily("Times new Roman"), FontSize = 16, HorizontalAlignment = HorizontalAlignment.Left };
            ButtonsInRightWindow[n].Content = s;
            RightWindowSection.Items.Add(ButtonsInRightWindow[n]);

            ButtonsInRightWindow[n].Click += BtnClick;
            n++;
            return n;
        }
    }
}