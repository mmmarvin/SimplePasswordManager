using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SimplePasswordManager.Custom;

namespace SimplePasswordManager.Dialogs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomActionSheet : CustomModalFrame
    {
        public CustomActionSheet(string[] action_list)
        {
            InitializeComponent();
            for(int i = 0; i < action_list.Length; ++i)
            {
                var action = action_list[i];
                var selection_label = new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, Text = action, TextColor = Theme.BACKGROUND_TEXT_COLOR, Padding = i == 0 ? new Thickness(0, 0, 0, 10) : new Thickness(0, 10, 0, 10) };
                var selection_label_tap_recognizer = new TapGestureRecognizer();
                selection_label_tap_recognizer.Tapped += (s, e) =>
                {
                    m_result.SetResult(selection_label.Text);
                };
                selection_label.GestureRecognizers.Add(selection_label_tap_recognizer);

                c_ActionList.Children.Add(selection_label);
            }
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