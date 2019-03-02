using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sharpnado.Presentation.Forms.CustomViews.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderlinedTabItem : TabItem
    {
        public UnderlinedTabItem()
        {
            LabelSize = 14;

            InitializeComponent();
        }
    }
}