using System.Threading.Tasks;
using Xamarin.Forms;

namespace SimplePasswordManager.Dialogs
{
    public class DialogFactory
    {
        public async static Task<string> DisplayPromptAsync(string title, string message, string ok = "OK", string cancel = "Cancel", bool can_be_empty = false, bool is_password = false)
        {
            var custom_prompt = new CustomPrompt() { PromptTitle = title, PromptMessage = message, PromptOKButtonText = ok, PromptCancelButtonText = cancel, PromptCanBeEmpty = can_be_empty, PromptIsPassword = is_password };
            await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
            var res = await custom_prompt.WaitForResult();
            custom_prompt.Close();

            return res;
        }

        public async static Task<string> DisplayPasswordPromptAsync(string title, string ok = "OK", string cancel = "Cancel")
        {
            var custom_prompt = new CustomPasswordPrompt() { PromptTitle = title, PromptOKButtonText = ok, PromptCancelButtonText = cancel };
            await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
            var res = await custom_prompt.WaitForResult();
            custom_prompt.Close();

            return res;
        }

        public async static Task<string> DisplayAlertAsync(string title, string message, string ok = "OK", string cancel = null)
        {
            var custom_prompt = new CustomAlert() { PromptTitle = title, PromptMessage = message, PromptOKButtonText = ok, PromptCancelButtonText = cancel };
            await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
            var res = await custom_prompt.WaitForResult();
            custom_prompt.Close();

            return res;
        }

        public async static Task<string> DisplayActionSheetAsync(string title, string cancel, string[] buttons)
        {
            var custom_prompt = new CustomActionSheet(buttons) { PromptTitle = title, PromptCancelButtonText = cancel };
            await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
            var res = await custom_prompt.WaitForResult();
            custom_prompt.Close();

            return res;
        }
    }
}
