using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Custom;

namespace SimplePasswordManager.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomAlert : CustomModalFrame
    {
        public CustomAlert()
        {
            InitializeComponent();
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
                if(value != null)
                {
                    c_CancelButton.Text = value;
                    c_CancelButton.IsVisible = true;
                }
            }
        }

        public void Btn_Clicked(object s, EventArgs e)
        {
            m_result.SetResult(((Button)s).Text);
        }

        public Task<string> WaitForResult()
        {
            m_result = new TaskCompletionSource<string>();
            return m_result.Task;
        }

        private TaskCompletionSource<string> m_result;
    }
}