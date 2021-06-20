using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventorySystem.Helpers
{
    internal class HyperlinkSpan : Span
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkSpan));

        public HyperlinkSpan()
        {
            TextDecorations = TextDecorations.Underline;
            TextColor = Color.LightSlateGray;
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Xamarin.Essentials.
                Command = new Command(async () => await Launcher.OpenAsync(Url))
            });
        }

        public string Url
        {
            get => (string) GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }
    }
}