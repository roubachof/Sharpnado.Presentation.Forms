using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DragAndDropSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();

            HorizontalListView.PreAnimation = (viewCell) =>
            {
                viewCell.View.Opacity = 0;
                viewCell.View.TranslationX = -25;
            };

            HorizontalListView.Animation = async (viewCell) =>
            {
                await Task.Delay(250)
                    .ContinueWith(async _ => viewCell.View.FadeTo(1))
                    .ContinueWith(async _ => viewCell.View.TranslateTo(0,0));
            };

            HorizontalListView.PostAnimation = async (viewCell) =>
            {
                await Task.Delay(250);
            };
        }
        
        protected override void OnAppearing()
            => (this.BindingContext as MainViewModel).OnStartCommand.Execute(null);

        private void EnableDyD(object sender, EventArgs e)
        {
            HorizontalListView.EnableDragAndDrop = !HorizontalListView.EnableDragAndDrop;
            LabelDragAndDrop.Text = HorizontalListView.EnableDragAndDrop.ToString();
        } 
    
    }
}