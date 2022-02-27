using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TestTask1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> rus_strings = new List<string>(); // List with russian strings
        List<double> rus_indexes = new List<double>(); // List with russian Petrenko's indexes for each row
        List<string> eng_strings = new List<string>(); // List with english strings
        List<double> eng_indexes = new List<double>(); // List with english Petrenko's indexes for each row
        int row = 0; // number of raw which contains now-checkable english row
        char[] separates = new char[] { ',', '.', ' ', ';', ':', '-', '—', '\'', '@', '!', '"', '#', '&',
                                        '№', '%', '$', '^', '*', '(', ')', '+', '=', '{', '}', '[', ']',
                                        '>', '<', '/', '~', '`', '|' }; // separates array
        public MainWindow()
        {
            InitializeComponent();
            InsideGRID.VerticalAlignment = VerticalAlignment.Top; /// alignment grid inside WPF-form
        }
        private void Up_Click(object sender, RoutedEventArgs e) // scrolling of scrollview event handler
        {
            scroll.LineUp(); // scroll Scrollview up
        }
        private void Down_Click(object sender, RoutedEventArgs e)// scrolling of scrollview event handler
        {
            scroll.LineDown(); // scroll Scrollview down
        }
        private void TextBlockAdd(List<string> strings, int i, int col) // method of adding a TextBlock in GRID
        {
            TextBlock text = new TextBlock(); // initializing a new TextBlock variable
            text.Text = strings[i]; // adding a text
            InsideGRID.Children.Add(text); // adding text in GRID
            Grid.SetRow(text, row); // setting a number of raw which will contain this text
            Grid.SetColumn(text, col); // setting a number of column which will contain this text
        }
        private void TextBlockAdd(List<double> strings, int i, int col) //overload of method TextBlockAdd for using List<double>
        {
            TextBlock text = new TextBlock(); // initializing a new TextBlock variable
            text.Text = strings[i].ToString(); // adding a text
            InsideGRID.Children.Add(text); // adding text in GRID
            Grid.SetRow(text, row); // setting a number of raw which will contain this text
            Grid.SetColumn(text, col); // setting a number of column which will contain this text
        }
        private void reading(List<string> strings, string link) // Method for reading data from file
        {
            using (StreamReader sr = new StreamReader(link)) // initializing new variable of StreamReader class to using it
            {
                string line; // now-readable row
                while ((line = sr.ReadLine()) != null) // read row while it isn't null
                {
                    strings.Add(line); // add to List
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e) // Clicking of button event handler
        {
            row = 0; // reset numbers for row's input
            reading(rus_strings, "rus.txt"); // reading russian rows
            reading(eng_strings, "eng.txt"); // reading english rows
            foreach(string str in rus_strings) // calculating amount of symbols in each russian row
            {
                int count = 0; // amount of symbols
                double index = 0; // total Petrenko's index of this row
                bool isSeparate = false; // is symbol separate or not
                foreach(char symbol in str) // checking of each symbols in raw
                {
                    isSeparate = false; // this symbol isn't separate yet
                    for (int i = 0; i < separates.Length; i++)
                    {
                        if (!isSeparate && symbol == separates[i]) // if this symbol isn't separate yet and if he separate now
                        {
                            isSeparate = true; // this symbol is separate
                        }
                    }
                    if (!isSeparate) count++; // if he not separate - add him to total amount of symbols
                }
                index = (count * count * count * 1.0) / 2; // calculating Petrenko's index - the sum of the arithmetic progression from 0,5 to last symbol
                                                           // with d = 1 multiply on count. index = ( S = 2 * 0,5 + 1 * (count - 1) / 2 * count ) * count =
                                                           // = (1 + count - 1)/ 2 * count * count = count * count * count / 2
                rus_indexes.Add(index); // add russian index into list
            }
            foreach (string str in eng_strings) // calculating amount of symbols in each english row with separating between comment
            {
                bool flag = false; // is now checkable symbol position before or after comment
                bool isSeparateBefore = false; // is founded separate before comment
                bool isSeparateAfter = false; // is founded separate before comment
                int count_before = 0; // count of symbols before comment
                int count_after = 0; // count of symbols after comment
                double index = 0; // index of english row
                foreach (char symbol in str) // for each symbol in english row
                {
                    isSeparateBefore = false; // this symbol isn't separate before comment yet
                    isSeparateAfter = false; // this symbol isn't separate after comment yet
                    for (int i = 0; i < separates.Length; i++)
                    {
                        if (!isSeparateBefore && !flag && symbol == separates[i]) isSeparateBefore = true; // if this symbol not separate before comment yet but it is really symbol before comment now
                        else if (!isSeparateAfter && flag && symbol == separates[i]) isSeparateAfter = true; // if this symbol not separate after comment yet but it is really symbol after comment now
                        else if (!flag && symbol == '|') flag = true; // if this symbol are comment's separate
                        else continue;
                    }
                    if (!isSeparateAfter && flag) count_after++; // counting a symbols after comment
                    if (!isSeparateBefore && !flag) count_before++; // counting a symbols before comment
                }
                index = (Math.Pow(count_after, 3) * 1.0 / 2) + (Math.Pow(count_before, 3) * 1.0 / 2); // counting an index of english row which equal sum of two Petrenko's index of rows before and after comment
                eng_indexes.Add(index); // adding indexes into list
            }

            for (int i = 0; i < rus_strings.Count * eng_strings.Count; i++)
            {
                InsideGRID.RowDefinitions.Add(new RowDefinition()); // adding place into GRID to new text fields
            }
            
            for (int i = 0; i < rus_strings.Count; i++)
            {     
                for(int j = 0; j < eng_indexes.Count; j++)
                {
                    if(rus_indexes[i] == eng_indexes[j]) // if Petrenko's indexes of english row and russian row are equal
                    {
                        row++; // incrementing row index
                        TextBlockAdd(rus_strings, i, 0); // add russian row in GRID
                        TextBlockAdd(eng_strings, i, 1); // add english row in GRID
                        TextBlockAdd(eng_indexes, i, 2); // add they's total index in GRID
                    }
                }
            }
            if (row == 0) MessageBox.Show("Отсутствуют строки с совпадающим индексом Петренко"); // if equal row are not exist show message about that
        }
    }
}
