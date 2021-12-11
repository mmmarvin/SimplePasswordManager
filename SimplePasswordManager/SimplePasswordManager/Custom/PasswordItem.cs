using Xamarin.Essentials;
using Xamarin.Forms;
using SimplePasswordManager.Dialogs;
using SimplePasswordManager.Pages;

namespace SimplePasswordManager.Custom
{
    public class PasswordItem : ContentView
    {
        private static readonly string PASSWORD_STRING = "************";

        public PasswordItem()
        {
            var grid = new Grid()
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition(),
                    new ColumnDefinition(),
                    new ColumnDefinition()
                }
            };
            grid.Children.Add(m_labelLabel, 0, 0);
            grid.Children.Add(m_usernameLabel, 1, 0);
            grid.Children.Add(m_passwordLabel, 2, 0);
            grid.Children.Add(m_passwordEntry, 2, 0);

            Content = new Frame()
            {
                BackgroundColor = Theme.BACKGROUND_COLOR,
                BorderColor = Theme.BACKGROUND_COLOR,
                CornerRadius = 0,
                Padding = new Thickness(10),
                Margin = new Thickness(0),
                Content = grid
            };

            m_Recognizer.Tapped += async (s, e) =>
            {
                var action = m_passwordLabel.IsVisible ? await DialogFactory.DisplayActionSheetAsync("Choose an Action", "Cancel", new string[]{ "Edit Entry", "Delete Entry", "Copy Label", "Copy Username", "Copy Password", "Show Password" }) :
                                                         await DialogFactory.DisplayActionSheetAsync("Choose an Action", "Cancel", new string[] { "Edit Entry", "Delete Entry", "Copy Label", "Copy Username", "Copy Password", "Hide Password" });
                if (action != null)
                {
                    if(action == "Edit Entry")
                    {
                        await Application.Current.MainPage.Navigation.PushModalAsync(new NewUser(Category, m_labelLabel.Text, m_usernameLabel.Text, m_password, Index));
                    }
                    else if(action == "Delete Entry")
                    {
                        var res = await DialogFactory.DisplayAlertAsync("Confirmation", "Are you sure you want to delete this entry?", "Yes", "No");
                        if(res != null && res == "Yes")
                        {
                            Global.PasswordManager.RemoveEntry(Category, Index);
                        }
                    }
                    else if(action == "Copy Label")
                    {
                        await Clipboard.SetTextAsync(m_labelLabel.Text);
                    }
                    else if(action == "Copy Username")
                    {
                        await Clipboard.SetTextAsync(m_usernameLabel.Text);
                    }
                    else if(action == "Copy Password")
                    {
                        await Clipboard.SetTextAsync(m_password);
                    }
                    else if(action == "Show Password")
                    {
                        IsPasswordVisible = true;
                    }
                    else if(action == "Hide Password")
                    {
                        IsPasswordVisible = false;
                    }
                }
            };
            GestureRecognizers.Add(m_Recognizer);
        }

        public string Label
        {
            get => m_labelLabel.Text;
            set => m_labelLabel.Text = value;
        }

        public string Username
        {
            get => m_usernameLabel.Text;
            set => m_usernameLabel.Text = value;
        }

        public string Password
        {
            get => m_password;
            set
            {
                m_password = value;
                if(m_passwordLabel.IsVisible)
                {
                    m_passwordLabel.Text = m_password.Length > 0 ? PASSWORD_STRING : "";
                }
                else
                {
                    m_passwordEntry.Text = m_password;
                }
            }
        }

        public bool IsPasswordVisible
        {
            get
            {
                return m_passwordEntry.IsVisible;
            }

            set
            {
                if(value)
                {
                    m_passwordLabel.IsVisible = false;
                    m_passwordLabel.Text = "";

                    m_passwordEntry.Text = m_password;
                    m_passwordEntry.IsVisible = true;
                }
                else
                {
                    m_passwordLabel.IsVisible = true;
                    m_passwordLabel.Text = m_password.Length > 0 ? PASSWORD_STRING : "";

                    m_passwordEntry.Text = "";
                    m_passwordEntry.IsVisible = false;
                }
            }
        }

        public string Category { get; set; }
        public int Index { get; set; }

        private Label                   m_labelLabel = new Label() { Text = "", TextColor = Theme.BACKGROUND_TEXT_COLOR, VerticalOptions = LayoutOptions.Center, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), Margin = new Thickness(10, 0, 0, 0), LineBreakMode = LineBreakMode.TailTruncation };
        private Label                   m_usernameLabel = new Label() { Text = "", TextColor = Theme.BACKGROUND_TEXT_COLOR, VerticalOptions = LayoutOptions.Center, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), Margin = new Thickness(0, 0, 0, 0), LineBreakMode = LineBreakMode.TailTruncation };
        private Label                   m_passwordLabel = new Label() { Text = "", TextColor = Theme.BACKGROUND_TEXT_COLOR, VerticalOptions = LayoutOptions.Center, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), Margin = new Thickness(0, 0, 10, 0), LineBreakMode = LineBreakMode.TailTruncation };
        private Entry                   m_passwordEntry = new Entry() { Text = "", TextColor = Theme.BACKGROUND_TEXT_COLOR, BackgroundColor = Theme.BACKGROUND_COLOR, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), Margin = new Thickness(0, 0, 10, 0), IsVisible = false, IsReadOnly = true };
        private string                  m_password;
        private TapGestureRecognizer    m_Recognizer = new TapGestureRecognizer();
    }
}
