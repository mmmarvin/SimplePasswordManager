using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Custom;
using SimplePasswordManager.Dialogs;
using System.IO;
using System.Reflection;
using System.Text;

namespace SimplePasswordManager.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeneratePassword : CustomModalFrame
    {
        public GeneratePassword(NewUser new_user_page)
        {
            InitializeComponent();
            m_newUserPage = new_user_page;
        }

        public void Picker_SchemeChanged(object s, EventArgs e)
        {
            var picker = (CustomPicker)s;
            if(picker.SelectedIndex == 0)
            {
                c_RandomLetterFrame.IsVisible = false;
                c_XKCDFrame.IsVisible = false;

                c_GenerateButton.IsEnabled = false;
                c_GenerateButton.BackgroundColor = Theme.BACKGROUND_COLOR;
            }
            else if(picker.SelectedIndex == 1)
            {
                c_RandomLetterFrame.IsVisible = true;
                c_XKCDFrame.IsVisible = false;

                c_GenerateButton.IsEnabled = true;
                c_GenerateButton.BackgroundColor = Theme.ACCENT_COLOR;
            }
            else if(picker.SelectedIndex == 2)
            {
                c_RandomLetterFrame.IsVisible = false;
                c_XKCDFrame.IsVisible = true;

                c_GenerateButton.IsEnabled = true;
                c_GenerateButton.BackgroundColor = Theme.ACCENT_COLOR;
            }
        }

        public async void Btn_GenerateClicked(object s, EventArgs e)
        {
            if (c_SchemePicker.SelectedIndex == 1)
            {
                char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
                char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                char[] symbols = { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', ':', ';', '\"', '\'', ',', '<', '.', '>', '/', '?', '\\', '[', '{', ']', '}', '~', ' ', '-', '_', '=', '+', '|' };

                bool use_letters = c_UseLettesSwitch.IsToggled;
                bool use_numbers = c_UseNumbersSwitch.IsToggled;
                bool use_symbols = c_UseSymbolSwitch.IsToggled;
                bool use_different_letter_cases = c_UseDifferentLetterCasesSwitch.IsToggled;

                if((use_letters || use_numbers || use_symbols) == false)
                {
                    await DialogFactory.DisplayAlertAsync("Error", "Must use atleast one of numbers, letters or symbols", "OK");
                    return;
                }

                var dictionary = new List<char>();
                if(use_letters)
                {
                    dictionary.AddRange(letters);
                }
                if(use_numbers)
                {
                    dictionary.AddRange(numbers);
                }
                if(use_symbols)
                {
                    dictionary.AddRange(symbols);
                }

                var password = new List<char>();
                var rng_generator = RandomNumberGenerator.Create();
                if(use_different_letter_cases)
                {
                    var number_in_bytes = new byte[sizeof(Int32)];
                    for(int i = 0, isize = int.Parse(c_LetterLengthEntry.Text); i < isize; ++i)
                    {
                        rng_generator.GetBytes(number_in_bytes);

                        var index = BitConverter.ToUInt16(number_in_bytes, 0) % dictionary.Count;
                        var sym = dictionary[index];
                        if(Char.IsLetter(sym))
                        {
                            rng_generator.GetBytes(number_in_bytes);
                            if(BitConverter.ToInt32(number_in_bytes, 0) % 2 == 0)
                            {
                                password.Add(Char.ToUpper(sym));
                            }
                            else
                            {
                                password.Add(sym);
                            }
                        }
                        else
                        {
                            password.Add(sym);
                        }
                    }
                }
                else
                {
                    var number_in_bytes = new byte[sizeof(Int32)];
                    for(int i = 0, isize = int.Parse(c_LetterLengthEntry.Text); i < isize; ++i)
                    {
                        rng_generator.GetBytes(number_in_bytes);

                        var index = BitConverter.ToUInt16(number_in_bytes, 0) % dictionary.Count;
                        password.Add(dictionary[index]);
                    }
                }

                m_newUserPage.Password = new string(password.ToArray());
            }
            else if (c_SchemePicker.SelectedIndex == 2)
            {
                bool capitalize_first_letter_of_word = c_CapitalizeFirstLetterSwitch.IsToggled;

                var dictionary = LoadWordDictionary();
                var password = new List<string>();

                var number_in_bytes = new byte[sizeof(Int32)];
                var rng_generator = RandomNumberGenerator.Create();
                for (int i = 0, isize = int.Parse(c_WordNumberEntry.Text); i < isize; ++i)
                {
                    if(i != 0)
                    {
                        password.Add(" ");
                    }

                    rng_generator.GetBytes(number_in_bytes);
                    var index = BitConverter.ToUInt16(number_in_bytes, 0) % dictionary.Count;

                    var word = dictionary[index];
                    if(capitalize_first_letter_of_word)
                    {
                        rng_generator.GetBytes(number_in_bytes);
                        if(BitConverter.ToUInt16(number_in_bytes, 0) % 2 == 0)
                        {
                            word = new string(Char.ToUpper(word[0]), 1) + word.Substring(1);
                        }
                    }
                    password.Add(word);
                }

                m_newUserPage.Password = CombineWords(password);
            }

            await Navigation.PopModalAsync(false);
        }

        public async void Btn_CancelClicked(object s, EventArgs e)
        {
            await Navigation.PopModalAsync(false);
        }

        private List<string> LoadWordDictionary()
        {
            var ret = new List<string>();
            using(var reader = new StreamReader(IntrospectionExtensions.GetTypeInfo(typeof(PasswordManager)).Assembly.GetManifestResourceStream("SimplePasswordManager.Resources.wordlist.txt")))
            {
                var line = reader.ReadLine();
                while(line != null)
                {
                    ret.Add(line);
                    line = reader.ReadLine();
                }
            }

            return ret;
        }

        private string CombineWords(List<string> words)
        {
            string ret = "";
            using(var ms = new MemoryStream())
            {
                using(var sw = new StreamWriter(ms, Encoding.ASCII))
                {
                    foreach(var word in words)
                    {
                        sw.Write(word);
                    }
                }
                ret = Encoding.ASCII.GetString(ms.ToArray());
            }

            return ret;
        }

        private NewUser m_newUserPage;
    }
}