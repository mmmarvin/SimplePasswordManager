using System;
using Xamarin.Forms;

namespace SimplePasswordManager.Custom
{
    public class PasswordStrengthView : ContentView
    {
        delegate bool CharComparisonFunction(char c);

        public PasswordStrengthView()
        {
            Content = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Label()
                            {
                                VerticalOptions = LayoutOptions.Center,
                                Text = "Password Strength: ",
                                TextColor = Theme.BACKGROUND_TEXT_COLOR
                            },
                            m_passwordStrengthLabel
                        }
                    },
                    m_passwordStrengthBoxView
                }
            };
        }

        public Entry PasswordEntry
        {
            set
            {
                m_entry = value;
                m_entry.TextChanged += (s, e) =>
                {
                    if (m_entry.Text.Length == 0)
                    {
                        m_passwordStrengthLabel.Text = "Very Weak";
                        m_passwordStrengthBoxView.BackgroundColor = Color.Red;
                        return;
                    }

                    var password = m_entry.Text;
                    var contains_letters = Contains(password, Char.IsLetter);
                    var contains_capital_letters = Contains(password, Char.IsUpper);
                    var contains_digits = Contains(password, Char.IsDigit);
                    var contains_symbols = Contains(password, Char.IsSymbol);
                    var contains_space = Contains(password, Char.IsWhiteSpace);

                    int n = 0;
                    if (contains_letters)
                    {
                        n += 26;
                    }
                    if(contains_capital_letters)
                    {
                        n += 26;
                    }
                    if (contains_digits)
                    {
                        n += 10;
                    }
                    if (contains_symbols)
                    {
                        n += 40;
                    }
                    if (contains_space)
                    {
                        n += 1;
                    }

                    double l = m_entry.Text.Length;
                    double h = l * Math.Log(n, 2);
                    if (h <= 28.0)
                    {
                        m_passwordStrengthLabel.Text = "Very Weak";
                        m_passwordStrengthBoxView.BackgroundColor = Color.Red;
                    }
                    else if (h > 28.0 && h <= 35.0)
                    {
                        m_passwordStrengthLabel.Text = "Weak";
                        m_passwordStrengthBoxView.BackgroundColor = Color.Red;
                    }
                    else if (h > 36.0 && h <= 59.0)
                    {
                        m_passwordStrengthLabel.Text = "Average";
                        m_passwordStrengthBoxView.BackgroundColor = Color.Yellow;
                    }
                    else if (h > 60.0 && h <= 127.0)
                    {
                        m_passwordStrengthLabel.Text = "Strong";
                        m_passwordStrengthBoxView.BackgroundColor = Color.Green;
                    }
                    else if (h > 128.0)
                    {
                        m_passwordStrengthLabel.Text = "Very Strong";
                        m_passwordStrengthBoxView.BackgroundColor = Color.Green;
                    }
                };
            }
        }

        private bool Contains(string str, CharComparisonFunction f)
        {
            foreach(var c in str)
            {
                if(f(c))
                {
                    return true;
                }
            }

            return false;
        }

        private Entry   m_entry = null;
        private Label   m_passwordStrengthLabel = new Label() { VerticalOptions = LayoutOptions.Center, Text = "Very Weak", TextColor = Theme.BACKGROUND_TEXT_COLOR };
        private BoxView m_passwordStrengthBoxView = new BoxView() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.Red, HeightRequest = 20 };
    }
}
