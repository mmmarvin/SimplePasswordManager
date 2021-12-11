using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Custom;
using SimplePasswordManager.Dialogs;

namespace SimplePasswordManager.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewUser : CustomModalFrame
    {
        delegate bool CharComparisonFunction(char c);

        public NewUser(string category)
        {
            InitializeComponent();
            c_TitleLabel.Text = "New Entry";
            c_AddButton.Text = "Add";

            c_AddButton.Clicked += async (s, e) =>
            {
                if (!await CheckFields())
                {
                    return;
                }

                Global.PasswordManager.AddEntry(category, c_LabelEntry.Text, c_UsernameEntry.Text, c_Password1Entry.Text);
                await Navigation.PopModalAsync(false);
            };

            Appearing += (s, e) =>
            {
                c_LabelEntry.Focus();
            };

            c_PasswordStrengthView.PasswordEntry = c_Password1Entry;
        }

        public NewUser(string category, string label, string username, string password, int index)
        {
            InitializeComponent();
            c_TitleLabel.Text = "Edit Entry";
            c_AddButton.Text = "Update";

            c_LabelEntry.Text = label;
            c_UsernameEntry.Text = username;
            c_Password1Entry.Text = password;
            c_Password2Entry.Text = password;
            c_AddButton.Clicked += async (s, e) =>
            {
                if(!await CheckFields())
                {
                    return;
                }

                Global.PasswordManager.UpdateEntry(category, index, c_LabelEntry.Text, c_UsernameEntry.Text, c_Password1Entry.Text);
                await Navigation.PopModalAsync(false);
            };

            Appearing += (s, e) =>
            {
                c_LabelEntry.Focus();
            };

            c_PasswordStrengthView.PasswordEntry = c_Password1Entry;
        }

        //public void Entry_PasswordChanged(object s, EventArgs e)
        //{
        //    bool Contains(string str, CharComparisonFunction f)
        //    {
        //        foreach(var c in str)
        //        {
        //            if(f(c))
        //            {
        //                return true;
        //            }
        //        }

        //        return false;
        //    }

        //    var password = c_Password1Entry.Text;
        //    var contains_letters = Contains(password, Char.IsLetter);
        //    var contains_digits = Contains(password, Char.IsDigit);
        //    var contains_symbols = Contains(password, Char.IsSymbol);
        //    var contains_space = Contains(password, Char.IsWhiteSpace);

        //    int n = 0;
        //    if(contains_letters)
        //    {
        //        n += 29;
        //    }
        //    if(contains_digits)
        //    {
        //        n += 10;
        //    }
        //    if(contains_symbols)
        //    {
        //        n += 26;
        //    }
        //    if(contains_space)
        //    {
        //        n += 1;
        //    }

        //    double l = c_Password1Entry.Text.Length;
        //    double h = l * (Math.Log(n) / Math.Log(2));
        //    if(h <= 5.0)
        //    {
        //        c_PasswordStrengthLabel.Text = "Weak";
        //        c_PasswordStrengthBox.BackgroundColor = Color.Red;
        //    }
        //    else if(h > 5.0 && h <= 10.0)
        //    {
        //        c_PasswordStrengthLabel.Text = "Average";
        //        c_PasswordStrengthBox.BackgroundColor = Color.Yellow;
        //    }
        //    else if(h > 10.0 && h <= 15.0)
        //    {
        //        c_PasswordStrengthLabel.Text = "Strong";
        //        c_PasswordStrengthBox.BackgroundColor = Color.Green;
        //    }
        //    else if(h > 15.0)
        //    {
        //        c_PasswordStrengthLabel.Text = "Very Strong";
        //        c_PasswordStrengthBox.BackgroundColor = Color.Green;
        //    }
        //}

        public async void Btn_GenerateClicked(object s, EventArgs e)
        {
            await Navigation.PushModalAsync(new GeneratePassword(this), false);
        }

        public void Btn_CancelClicked(object s, EventArgs e)
        {
            Close();
        }

        public string Password
        {
            set
            {
                c_Password1Entry.Text = value;
                c_Password2Entry.Text = value;
            }
        }

        private async Task<bool> CheckFields()
        {
            if (c_LabelEntry.Text.Length == 0)
            {
                c_LabelEntry.Focus();
                return false;
            }
            if (c_UsernameEntry.Text.Length == 0)
            {
                c_UsernameEntry.Focus();
                return false;
            }
            if (c_Password1Entry.Text != c_Password2Entry.Text)
            {
                await DialogFactory.DisplayAlertAsync("Error", "Password does not match", "OK");
                c_Password2Entry.Focus();
                return false;
            }

            return true;
        }
    }
}