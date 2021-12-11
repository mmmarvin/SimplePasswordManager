using Xamarin.Forms;

namespace SimplePasswordManager.Custom
{
    public class PopupFrame : Frame
    {
        public PopupFrame()
        {
        }

        public new bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }

            set
            {
                if(!base.IsVisible && value)
                {
                    base.IsVisible = true;
                    AnchorX = 1;
                    AnchorY = 0.1;

                    Animation scale_animation = new Animation(f => Scale = f, 0.5, 1, Easing.SinInOut);
                    Animation fade_animation = new Animation(f => Opacity = f, 0.2, 1, Easing.SinInOut);

                    scale_animation.Commit(this, "popup_scale_animation", 250);
                    fade_animation.Commit(this, "popup_fade_animation", 250);
                }
                else
                {
                    base.IsVisible = value;
                }
            }
        }
    }
}
