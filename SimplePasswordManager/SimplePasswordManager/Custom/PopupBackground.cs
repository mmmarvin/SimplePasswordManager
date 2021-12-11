using Xamarin.Forms;

namespace SimplePasswordManager.Custom
{
    public class PopupBackground : Frame
    {
        public PopupBackground()
        {
            BackgroundColor = Color.Transparent;
            Padding = new Thickness(0);
            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0, 0, 1, 1));

            m_tapRecognizer.Tapped += (s, e) =>
            {
                IsVisible = false;
            };

            GestureRecognizers.Add(m_tapRecognizer);
        }

        public PopupFrame Popup
        {
            get
            {
                return m_popupFrame;
            }

            set
            {
                m_popupFrame = value;
            }
        }

        public new bool IsVisible
        {
            get
            {
                if(m_popupFrame != null)
                {
                    return base.IsVisible && m_popupFrame.IsVisible;
                }

                return base.IsVisible;
            }

            set
            {
                base.IsVisible = value;
                if(m_popupFrame != null)
                {
                    m_popupFrame.IsVisible = value;
                }
            }
        }

        private PopupFrame              m_popupFrame = null;
        private TapGestureRecognizer    m_tapRecognizer = new TapGestureRecognizer();
    }
}
