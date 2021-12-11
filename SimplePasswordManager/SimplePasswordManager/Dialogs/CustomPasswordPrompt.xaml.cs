using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Custom;

namespace SimplePasswordManager.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomPasswordPrompt : CustomModalFrame
    {
        public CustomPasswordPrompt()
        {
            InitializeComponent();
            Appearing += (s, e) =>
            {
                c_Password1Entry.Focus();
            };

            c_PasswordStrengthView.PasswordEntry = c_Password1Entry;
        }

        public string PromptTitle
        {
            get
            {
                return c_TitleLabel.Text;
            }

            set
            {
                c_TitleLabel.Text = value;
            }
        }

        public string PromptOKButtonText
        {
            get
            {
                return c_OKButton.Text;
            }

            set
            {
                c_OKButton.Text = value;
            }
        }

        public string PromptCancelButtonText
        {
            get
            {
                return c_CancelButton.Text;
            }

            set
            {
                c_CancelButton.Text = value;
            }
        }

        public async void Btn_OKClicked(object s, EventArgs e)
        {
            if(c_Password1Entry.Text == c_Password2Entry.Text)
            {
                m_result.SetResult(c_Password1Entry.Text);
            }
            else
            {
                await DialogFactory.DisplayAlertAsync("Error", "Password does not match", "OK");
                c_Password1Entry.Focus();
            }
        }

        public void Btn_CancelClicked(object s, EventArgs e)
        {
            m_result.SetResult(null);
        }

        public Task<string> WaitForResult()
        {
            m_result = new TaskCompletionSource<string>();
            return m_result.Task;
        }

        private TaskCompletionSource<string> m_result;
    }
}