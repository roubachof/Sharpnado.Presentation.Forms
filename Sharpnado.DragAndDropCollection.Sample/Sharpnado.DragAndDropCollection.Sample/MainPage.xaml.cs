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
        }

        protected override void OnAppearing()
            => (this.BindingContext as MainViewModel).OnStartCommand.Execute(null);
    }
}