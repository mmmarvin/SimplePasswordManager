using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using SimplePasswordManager.Utility;

namespace SimplePasswordManager.Custom
{
    public class CustomPicker : Frame
    {
        private class LabelWithIndex : Label
        {
            public int Index { get; set; }
        }

        private class IndexEvent : EventArgs
        {
            public int Index { get; set; }
        }

        private class IndexSelectionDialog : CustomModalFrame
        {
            public IndexSelectionDialog(IList<string> selections)
            {
                for(int i = 0; i < selections.Count; ++i)
                {
                    var selection = selections[i];
                    var selection_label = new LabelWithIndex() { HorizontalOptions = LayoutOptions.FillAndExpand, Text = selection, TextColor = Theme.BACKGROUND_TEXT_COLOR, Padding = i == 0 ? new Thickness(0, 0, 0, 10) : new Thickness(0, 10, 0, 10), Index = i, LineBreakMode = LineBreakMode.TailTruncation };
                    var selection_label_tap_recognizer = new TapGestureRecognizer();
                    selection_label_tap_recognizer.Tapped += (s, e) =>
                    {
                        Btn_SelectionClicked(selection_label, new IndexEvent() { Index = selection_label.Index });
                    };
                    selection_label.GestureRecognizers.Add(selection_label_tap_recognizer);

                    Children.Add(selection_label);
                }

                var cancel_button = new Button()
                {
                    Text = "Cancel",
                    TextColor = Theme.BACKGROUND_TEXT_COLOR,
                    BackgroundColor = Theme.BACKGROUND_COLOR,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Button)),
                    Margin = new Thickness(0, 10, 0, 0)
                };
                cancel_button.Clicked += Btn_CancelClicked;

                var button_layout = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        },
                        cancel_button
                    }
                };

                Children.Add(button_layout);
            }

            public void Btn_SelectionClicked(object s, EventArgs e)
            {
                m_result.SetResult(((IndexEvent)e).Index);
            }

            public void Btn_CancelClicked(object s, EventArgs e)
            {
                m_result.SetResult(-2);
            }

            public Task<int> WaitForResult()
            {
                m_result = new TaskCompletionSource<int>();
                return m_result.Task;
            }

            private TaskCompletionSource<int> m_result;
        }

        public CustomPicker()
        {
            BorderColor = Theme.BACKGROUND_TEXT_COLOR;
            HasShadow = false;
            Padding = new Thickness(0);

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += async (s, e) =>
            {
                var custom_prompt = new IndexSelectionDialog(Items);
                await Application.Current.MainPage.Navigation.PushModalAsync(custom_prompt, false);
                var res = await custom_prompt.WaitForResult();
                custom_prompt.Close();

                if(res != -2)
                {
                    SelectedIndex = res;
                }
            };

            Content = m_label;
            GestureRecognizers.Add(tapGestureRecognizer);
        }

        public event EventHandler SelectedIndexChanged
        { 
            add
            {
                m_selectionIndexChanged.Add(value);
            }

            remove
            {
                m_selectionIndexChanged.Remove(value);
            }
        }

        public IList<string> Items { get; } = new List<string>();

        public int SelectedIndex 
        { 
            get
            {
                return m_selectedIndex;
            }

            set
            {
                m_selectedIndex = value;
                if(m_selectedIndex != -1)
                {
                    m_label.Text = Items[m_selectedIndex];
                }

                foreach(var e in m_selectionIndexChanged)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        public Color TextColor
        {
            get
            {
                return m_label.TextColor;
            }

            set
            {
                m_label.TextColor = value;
            }
        }

        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get
            {
                return m_label.FontSize;
            }

            set
            {
                m_label.FontSize = value;
            }
        }

        private int                 m_selectedIndex = -1;
        private Label               m_label = new Label() { Margin = new Thickness(5, 10, 5, 10), LineBreakMode = LineBreakMode.TailTruncation  };
        private List<EventHandler>  m_selectionIndexChanged = new List<EventHandler>();
    }
}
