using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Input;
using Sharpnado.Infrastructure.Tasks;
using Xamarin.Forms;

namespace Sharpnado.Presentation.Forms.ViewModels
{
    public interface IViewModelLoader
    {
        ICommand ReloadCommand { get; }

        ICommand RefreshCommand { get; }

        bool IsCompleted { get; }

        bool IsNotStarted { get; }

        bool IsNotCompleted { get; }

        bool IsSuccessfullyCompleted { get; }

        bool IsCanceled { get; }

        bool IsFaulted { get; }

        Exception Error { get; }

        string ErrorMessage { get; }

        string EmptyStateMessage { get; }

        bool ShowLoader { get; }

        bool ShowRefresher { get; }

        bool ShowResult { get; }

        bool ShowError { get; }

        bool ShowEmptyState { get; }

        bool ShowErrorNotification { get; }
    }

    public abstract class ViewModelLoaderBase : Bindable, IViewModelLoader
    {
        private readonly Func<Exception, string> _errorHandler;

        private bool _showLoader;
        private bool _showRefresher;
        private bool _showResult;
        private bool _showError;
        private bool _showEmptyState;
        private bool _showErrorNotification;
        private Exception _error;
        private string _errorMessage;
        private string _emptyStateMessage;

        protected ViewModelLoaderBase(Func<Exception, string> errorHandler = null, string emptyStateMessage = null)
        {
            _errorHandler = errorHandler ?? DefaultErrorHandler;

            EmptyStateMessage = emptyStateMessage;
        }

        public ICommand ReloadCommand { get; protected set; }

        public ICommand RefreshCommand { get; protected set; }

        public bool IsCompleted => CurrentLoadingTask.IsCompleted;

        public abstract bool IsNotStarted { get; }

        public bool IsNotCompleted => CurrentLoadingTask.IsNotCompleted;

        public bool IsSuccessfullyCompleted => CurrentLoadingTask.IsSuccessfullyCompleted;

        public bool IsCanceled => CurrentLoadingTask.IsCanceled;

        public bool IsFaulted => CurrentLoadingTask.IsFaulted;

        public bool ShowLoader
        {
            get => _showLoader;
            set => SetAndRaise(ref _showLoader, value);
        }

        public bool ShowRefresher
        {
            get => _showRefresher;
            set => SetAndRaise(ref _showRefresher, value);
        }

        public bool ShowResult
        {
            get => _showResult;
            set => SetAndRaise(ref _showResult, value);
        }

        public bool ShowError
        {
            get => _showError;
            set => SetAndRaise(ref _showError, value);
        }

        public bool ShowEmptyState
        {
            get => _showEmptyState;
            set => SetAndRaise(ref _showEmptyState, value);
        }

        public bool ShowErrorNotification
        {
            get => _showErrorNotification;
            set => SetAndRaise(ref _showErrorNotification, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetAndRaise(ref _errorMessage, value);
        }

        public Exception Error
        {
            get => _error;
            set => SetAndRaise(ref _error, value);
        }

        public string EmptyStateMessage
        {
            get => _emptyStateMessage;
            set => SetAndRaise(ref _emptyStateMessage, value);
        }

        protected object SyncRoot { get; } = new object();

        protected INotifyTask CurrentLoadingTask { get; set; }

        protected void OnTaskCompleted(INotifyTask task)
        {
            // Log.Info("Task completed");
            ShowRefresher = ShowLoader = false;

            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsNotStarted));
        }

        protected void OnTaskFaulted(INotifyTask faultedTask, bool isRefreshing)
        {
            // Log.Info("Task completed with fault");
            RaisePropertyChanged(nameof(IsFaulted));

            ShowError = !isRefreshing;
            ShowErrorNotification = isRefreshing;
            Error = faultedTask.InnerException;
            ErrorMessage = ToErrorMessage(faultedTask.InnerException);
        }

        protected virtual void OnTaskSuccessfullyCompleted(INotifyTask task)
        {
            // Log.Info("Task successfully completed");
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            ShowResult = true;
        }

        protected string ToErrorMessage(Exception exception)
        {
            return _errorHandler.Invoke(exception);
        }

        protected virtual void Reset(bool isRefreshing)
        {
            ShowLoader = !isRefreshing;
            ShowRefresher = isRefreshing;

            if (!isRefreshing)
            {
                Error = null;
                ShowError = ShowResult = ShowEmptyState = false;
            }

            RaisePropertyChanged(nameof(IsCompleted));
            RaisePropertyChanged(nameof(IsNotCompleted));
            RaisePropertyChanged(nameof(IsNotStarted));
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));
            RaisePropertyChanged(nameof(IsFaulted));
        }

        private static string DefaultErrorHandler(Exception exception)
        {
            return "An unknown error occured";
        }
    }

    public class ViewModelLoader : ViewModelLoaderBase
    {
        private Func<Task> _loadingTaskSource;

        public ViewModelLoader(Func<Exception, string> errorHandler = null, string emptyStateMessage = null)
            : base(errorHandler, emptyStateMessage)
        {
            CurrentLoadingTask = NotifyTask.NotStartedTask;
            ReloadCommand = new Command(() => Load(_loadingTaskSource));
            RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));
        }

        public override bool IsNotStarted => CurrentLoadingTask == NotifyTask.NotStartedTask;

        public void Load(Func<Task> loadingTaskSource, bool isRefreshing = false)
        {
            // Log.Info("Load");
            lock (SyncRoot)
            {
                if (CurrentLoadingTask != NotifyTask.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    // Log.Warn("A loading task is currently running: discarding this call");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new NotifyTask.Builder(_loadingTaskSource)
                    .WithWhenCompleted(OnTaskCompleted)
                    .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                    .WithWhenSuccessfullyCompleted(OnTaskSuccessfullyCompleted)
                    .Build();
            }

            Reset(isRefreshing);
            CurrentLoadingTask.Start();
        }
    }

    public class ViewModelLoader<TData> : ViewModelLoaderBase
        where TData : class
    {
        private Func<Task<TData>> _loadingTaskSource;

        private TData _result;

        public ViewModelLoader(Func<Exception, string> errorHandler = null, string emptyStateMessage = null)
            : base(errorHandler, emptyStateMessage)
        {
            CurrentLoadingTask = NotifyTask<TData>.NotStartedTask;
            ReloadCommand = new Command(() => Load(_loadingTaskSource));
            RefreshCommand = new Command(() => Load(_loadingTaskSource, isRefreshing: true));
        }

        public override bool IsNotStarted => CurrentLoadingTask == NotifyTask<TData>.NotStartedTask;

        public TData Result
        {
            get => _result;
            set => SetAndRaise(ref _result, value);
        }

        public void Load(Func<Task<TData>> loadingTaskSource, bool isRefreshing = false)
        {
            // Log.Info("Load");
            lock (SyncRoot)
            {
                if (CurrentLoadingTask != NotifyTask<TData>.NotStartedTask && CurrentLoadingTask.IsNotCompleted)
                {
                    // Log.Warn("A loading task is currently running: discarding this call");
                    return;
                }

                if (CurrentLoadingTask == NotifyTask<TData>.NotStartedTask && loadingTaskSource == null)
                {
                    // Log.Warn("Refresh requested while not loaded yet, aborting...");
                    return;
                }

                _loadingTaskSource = loadingTaskSource;

                CurrentLoadingTask = null;
                CurrentLoadingTask = new NotifyTask<TData>.Builder(_loadingTaskSource)
                    .WithWhenCompleted(OnTaskCompleted)
                    .WithWhenFaulted(faultedTask => OnTaskFaulted(faultedTask, isRefreshing))
                    .WithWhenSuccessfullyCompleted(
                        (completedTask, result) =>
                            {
                                Result = result;
                                OnTaskSuccessfullyCompleted(completedTask);
                            })
                    .Build();
            }

            Reset(isRefreshing);
            CurrentLoadingTask.Start();
        }

        protected override void Reset(bool isRefreshing)
        {
            base.Reset(isRefreshing);

            RaisePropertyChanged(nameof(Result));
        }

        protected override void OnTaskSuccessfullyCompleted(INotifyTask task)
        {
            // Log.Info("Task successfully completed");
            RaisePropertyChanged(nameof(IsSuccessfullyCompleted));

            if (EmptyStateMessage != null && (Result == null || (Result is ICollection collection && collection.Count == 0)))
            {
                // Empty state message is specified: show empty state view
                // Log.Info("Showing empty state");
                ShowEmptyState = true;
                return;
            }

            ShowResult = true;
        }
    }
}
