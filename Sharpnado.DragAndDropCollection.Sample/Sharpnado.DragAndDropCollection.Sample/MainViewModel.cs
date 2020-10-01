using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sharpnado.DragAndDropCollection.Sample
{
    public class MainViewModel : BindableObject
    {
        public MainViewModel()
        {
            StartDragCommand = new Command(StartDragHandler);
            EndDragCommand = new Command(EndDragHandler);
            OnStartCommand = new Command(OnStartHandler);
        }
        
        private ObservableCollection<DummyData> _data;

        public ObservableCollection<DummyData> Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }



        public ICommand StartDragCommand
        {
            get;
            private set;
        }

        public ICommand EndDragCommand
        {
            get;
            private set;
        }

        public ICommand OnStartCommand
        {
            get;
            private set;
        }

        
        private void StartDragHandler(object obj)
        {
            
        }
        
        private void EndDragHandler(object obj)
        {
            
        }
        
        private void OnStartHandler(object obj)
            => Data = new ObservableCollection<DummyData>(CreateDummyData());
        
        private IEnumerable<DummyData> CreateDummyData()
        {
            var data = new List<DummyData>();
            for (var i = 0; i < 25; i++)
            {
                data.Add(new DummyData()
                {
                    Order = i,
                    Title = i.ToString()
                });
            }

            return data;
        }
    }
    
    

    public class DummyData
    {
        public string Title { get; set; }

        public int Order { get; set; }
    }
}