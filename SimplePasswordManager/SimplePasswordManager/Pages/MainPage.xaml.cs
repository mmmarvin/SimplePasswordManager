using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using SimplePasswordManager.Custom;
using SimplePasswordManager.Dialogs;

namespace SimplePasswordManager.Pages
{
    public partial class MainPage : ContentPage, IObserver
    {
        public MainPage() 
        {
            InitializeComponent();
            c_OptionsButton.Source = ImageSource.FromResource("SimplePasswordManager.Resources.options_icon.png");

            Appearing += OnFocus;
            Disappearing += OnUnFocus;

            c_PopupBackground.Popup = c_PopupFrame;
        }

        public void Close()
        {
            Process.GetCurrentProcess().Kill();
        }

        public async void OnFocus(object s, EventArgs e)
        {
            if(Settings.FirstRun)
            {
                Settings.FirstRun = false;

                // show first run dialogs
                await DialogFactory.DisplayAlertAsync("Disclaimer", "THIS SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.", "Continue");
            }

            if (Global.PasswordManager == null)
            {
                Global.PasswordManager = new PasswordManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "passwords.dat"));
                Global.PasswordManager.AddObserver(this);

                var load_result = Global.PasswordManager.Load();
                if(load_result == PasswordManager.ELoadResult.NeedPassword)
                {
                    var load_success = false;
                    var i = 0;
                    while(i < 3)
                    {
                        var master_password = await DialogFactory.DisplayPromptAsync("Prompt", "Enter the master password", "OK", "Cancel", false, true);
                        if(master_password == null)
                        {
                            await DialogFactory.DisplayAlertAsync("Error", "Cannot decrypt the database", "OK");
                            Close();
                            return;
                        }

                        Global.PasswordManager.SetMasterPassword(master_password);
                        load_result = Global.PasswordManager.Load();
                        if(load_result == PasswordManager.ELoadResult.Success)
                        {
                            load_success = true;
                            break;
                        }
                        else if(load_result == PasswordManager.ELoadResult.InvalidPassword)
                        {
                            await DialogFactory.DisplayAlertAsync("Error", "Invalid master password!", "OK");
                            ++i;
                        }
                        else
                        {
                            await DialogFactory.DisplayAlertAsync("Error", "An unknown error has occured. Exiting...", "OK");
                            Close();
                            return;
                        }
                    }

                    if(!load_success)
                    {
                        await DialogFactory.DisplayAlertAsync("Error", "Exceeded maximum tries", "OK");
                        Close();
                        return;
                    }
                }
                else if(load_result == PasswordManager.ELoadResult.FileDoesNotExist)
                {
                    c_CategoryPicker.Items.Add("Choose a category...");
                    c_CategoryPicker.SelectedIndex = 0;
                }
                else
                {
                    if (load_result != PasswordManager.ELoadResult.Success)
                    {
                        await DialogFactory.DisplayAlertAsync("Error", load_result.ToString(), "OK");
                        return;
                    }
                }
            }
        }

        public void OnUnFocus(object s, EventArgs e)
        {
            //Global.PasswordManager = null;
            //GC.Collect();
        }

        public async void Btn_MasterPasswordClicked(object s, EventArgs e)
        {
            async void ReadMasterPassword()
            {
                // ask for a master password
                var master_password = await DialogFactory.DisplayPasswordPromptAsync("Master Password");
                if (master_password != null)
                {
                    if (!Global.PasswordManager.SetMasterPassword(master_password))
                    {
                        await DialogFactory.DisplayAlertAsync("Error", "There was an error setting the master password", "OK");
                    }
                    else
                    {
                        if (Global.PasswordManager.Count > 0)
                        {
                            if (!Global.PasswordManager.Save())
                            {
                                await DialogFactory.DisplayAlertAsync("Error", "There was an error setting the master password", "OK");
                            }
                        }
                    }
                }
            }

            // hide the popup
            c_PopupBackground.IsVisible = false;
            if(Global.PasswordManager.IsPasswordSet)
            {
                ReadMasterPassword();
            }
            else
            {
                var res = await DialogFactory.DisplayAlertAsync("Warning", "While encrypting the password database will protect it from unwanted access, encryption may cause the database to be unrecoverable, if you forget the password. Continue?", "Yes", "No");
                if (res != null && res == "Yes")
                {
                    ReadMasterPassword();
                }
            }
        }

        public async void Btn_PrivacyClicked(object s, EventArgs e)
        {
            c_PopupBackground.IsVisible = false;
            await Global.NavigationPage.PushAsync(new FilePage(false, "https://completefitness.github.io/MyPasswordManager/privacy.html") { Title = "Privacy Policy" });
        }

        public async void Btn_AboutClicked(object s, EventArgs e)
        {
            c_PopupBackground.IsVisible = false;
            await Global.NavigationPage.PushAsync(new About());
        }

        public void Btn_PopupClicked(object s, EventArgs e)
        {
            if(!c_PopupBackground.IsVisible)
            {
                c_PopupBackground.IsVisible = true;
            }
        }

        public async void Btn_NewCategoryClicked(object s, EventArgs e)
        {
            var res = await DialogFactory.DisplayPromptAsync("New Category", "Enter a new category name", "OK", "Cancel");
            if(res != null)
            {
                if(!Global.PasswordManager.CreateNewCategory(res))
                {
                    await DialogFactory.DisplayAlertAsync("Error", "There was an error adding a new category", "OK");
                }
            }
        }

        public async void Btn_DeleteCategoryClicked(object s, EventArgs e)
        {
            var res = await DialogFactory.DisplayAlertAsync("Confirmation", string.Format("Are you sure you want to delete \"{0}\"?", c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex]), "Yes", "No");
            if(res != null && res == "Yes")
            {
                if(!Global.PasswordManager.RemoveCategory(c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex]))
                {
                    await DialogFactory.DisplayAlertAsync("Error", "There was an error removing the category", "OK");
                }
            }
        }

        public async void Btn_AddUserClicked(object s, EventArgs e)
        {
            await Navigation.PushModalAsync(new NewUser(c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex]), false);
        }

        public void Picker_CategoryChanged(object s, EventArgs e)
        {
            if(m_updateList)
            {
                if(c_CategoryPicker.SelectedIndex > 0)
                {
                    var password_list = Global.PasswordManager.GetUsernameAndPasswords(c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex]);
                    c_PasswordList.Children.Clear();
                    foreach(var usernamepassword in password_list)
                    {
                        var index = c_PasswordList.Children.Count;
                        c_PasswordList.Children.Add(new PasswordItem() { Category = c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex], Label = usernamepassword.Label, Username = usernamepassword.Username, Password = usernamepassword.Password, Index = index });
                    }
                }
                else
                {
                    c_PasswordList.Children.Clear();
                }
            }
            
            if(c_CategoryPicker.SelectedIndex <= 0)
            {
                c_AddUserButton.IsEnabled = false;
                c_AddUserButton.BackgroundColor = Theme.BACKGROUND_COLOR;
                c_DeleteCategoryButton.IsEnabled = false;
            }
            else
            {
                c_AddUserButton.IsEnabled = true;
                c_AddUserButton.BackgroundColor = Theme.ACCENT_COLOR;

                c_DeleteCategoryButton.IsEnabled = true;
            }
        }

        public void OnNotify(object s, string e, object p)
        {
            void RefillCategoryPicker()
            {
                c_CategoryPicker.Items.Clear();
                c_CategoryPicker.Items.Add("Choose a category...");
                foreach(var item in Global.PasswordManager.GetCategoryList())
                {
                    c_CategoryPicker.Items.Add(item);
                }
            }

            if(e == "Load")
            {
                RefillCategoryPicker();
                c_CategoryPicker.SelectedIndex = 0;
            }
            else if(e == "AddCategory")
            {
                //m_updateList = false;
                var ev = (CategoryEvent)p;
                RefillCategoryPicker();
                for(int i = 0; i < c_CategoryPicker.Items.Count; ++i)
                {
                    if(c_CategoryPicker.Items[i] == ev.Category)
                    {
                        c_CategoryPicker.SelectedIndex = i;
                        //m_updateList = true;
                        break;
                    }
                }
                //m_updateList = true;
            }
            else if(e == "RemoveCategory")
            {
                var chosen_category = c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex];

                m_updateList = false;
                if(((CategoryEvent)p).Category == chosen_category)
                {
                    RefillCategoryPicker();
                    c_CategoryPicker.SelectedIndex = 0;
                    c_PasswordList.Children.Clear();
                }
                else
                {
                    RefillCategoryPicker();
                    for(int i = 0; i < c_CategoryPicker.Items.Count; ++i)
                    {
                        if(c_CategoryPicker.Items[i] == chosen_category)
                        {
                            c_CategoryPicker.SelectedIndex = i;
                            break ;
                        }
                    }
                }
                m_updateList = true;
            }
            else 
            {
                var chosen_category = c_CategoryPicker.Items[c_CategoryPicker.SelectedIndex];
                var ev = (PasswordEvent)p;
                if(chosen_category == ev.Category)
                {
                    if(e == "AddPassword")
                    {
                        var index = c_PasswordList.Children.Count;
                        c_PasswordList.Children.Add(new PasswordItem() { Category = ev.Category, Label = ev.UsernamePassword.Label, Username = ev.UsernamePassword.Username, Password = ev.UsernamePassword.Password, Index = index });
                    }
                    else if(e == "UpdatePassword")
                    {
                        var uev = (PasswordUpdateEvent)p;
                        var item = (PasswordItem)c_PasswordList.Children[uev.Index];

                        item.Label = uev.UsernamePassword.Label;
                        item.Username = uev.UsernamePassword.Username;
                        item.Password = uev.UsernamePassword.Password;
                    }
                    else if(e == "RemovePassword")
                    {
                        var uev = (PasswordUpdateEvent)p;
                        c_PasswordList.Children.RemoveAt(uev.Index);
                    }
                }
            }
        }

        //private void ClosePasswordManager()
        //{
        //    Global.PasswordManager = null;

        //    // clear the category picker
        //    c_CategoryPicker.Items.Clear();
        //    c_CategoryPicker.Items.Add("Choose a category...");
        //    c_CategoryPicker.SelectedIndex = 0;

        //    // clear the password list
        //    c_PasswordList.Children.Clear();

        //    GC.Collect();
        //}

        private bool            m_updateList = true;
    }
}
