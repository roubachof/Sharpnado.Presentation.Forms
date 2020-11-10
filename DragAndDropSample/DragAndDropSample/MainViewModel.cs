using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Xamarin.Forms;

namespace DragAndDropSample
{
    public class MainViewModel : BindableObject
    {
        public MainViewModel()
        {
            StartDragCommand = new Command(StartDragHandler);
            EndDragCommand = new Command(EndDragHandler);
            OnStartCommand = new Command(OnStartHandler);
            ScrollCurrentCommand = new Command(CurrentScrollHandler);
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

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
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

        public ICommand ScrollCurrentCommand
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
        
        private void CurrentScrollHandler(object obj)
        {
            CurrentIndex = (int) obj;
        }
        
        private IEnumerable<DummyData> CreateDummyData()
        {
            var data = new List<DummyData>();
            for (var i = 0; i < 100; i++)
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