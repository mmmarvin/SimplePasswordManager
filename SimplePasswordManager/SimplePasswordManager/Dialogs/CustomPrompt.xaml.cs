using System;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Custom;

namespace SimplePasswordManager.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomPrompt : CustomModalFrame
    {
        public CustomPrompt()
        {
            InitializeComponent();
            Appearing += (s, e) =>
            {
                c_ValueEntry.Focus();
            };
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

        public string PromptMessage
        {
            get
            {
                return c_MessageLabel.Text;
            }

            set
            {
                c_MessageLabel.Text = value;
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
                if(value != null && value.Length > 0)
                {
                    c_CancelButton.Text = value;
                    c_CancelButton.IsVisible = true;
                }
                else
                {
                    c_CancelButton.Text = "";
                    c_CancelButton.IsVisible = false;
                }
            }
        }

        public bool PromptIsPassword
        {
            get
            {
                return c_ValueEntry.IsPassword;
            }

            set
            {
                c_ValueEntry.IsPassword = value;
            }
        }

        public bool PromptCanBeEmpty { get; set; }

        public void Btn_OKClicked(object s, EventArgs e)
        {
            if(PromptCanBeEmpty)
            {
                m_result.SetResult(c_ValueEntry.Text);
            }
            else
            {
                if(c_ValueEntry.Text.Length == 0)
                {
                    c_ValueEntry.Focus();
                }
                else
                {
                    m_result.SetResult(c_ValueEntry.Text);
                }
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

        private TaskCompletionSource<string>    m_result;
    }
}