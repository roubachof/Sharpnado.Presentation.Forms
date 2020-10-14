using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sharpnado.DragAndDropCollection.Sample
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
    }
}