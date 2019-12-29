using Caliburn.Micro;
using Resxul.Properties;

namespace Resxul.ViewModels
{
    internal sealed class DialogViewModel : Screen
    {
        public DialogViewModel()
        {
            DisplayName = Global.Name;

            YesButtonText = Resources.Yes;
            NoButtonText = Resources.No;

            DialogWidth = 520;

            if (View.InDesignMode)
            {
                Title = "Turpis pid";
                Message = 
                    "Placerat ac lacus velit, auctor augue sit, nec, odio, placerat purus hac montes lundium enim a rhoncus in phasellus odio elit in aliquet. " +
                    "Turpis pid, amet, urna duis nec nunc adipiscing rhoncus proin pulvinar hac! Vut augue porttitor tristique, magnis elementum? Rhoncus! " +
                    "Tempor, placerat? Lacus non ultricies? Porttitor, vel cum, amet, dapibus nisi est est sed elit vut integer, tortor et sed.";
            }
        }

        //

        public double DialogWidth { get; set; }
        public bool HideIcon { get; set; }
        public bool HideYesButton { get; set; }
        public bool HideNoButton { get; set; }
        public bool HideTitle { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string YesButtonText { get; set; }
        public string NoButtonText { get; set; }
    }
}
