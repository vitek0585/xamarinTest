using System.Threading.Tasks;
using System.Windows.Input;
using Android.Content;
using Android.Content.PM;
using Xamarin.Forms;

namespace XamarinForm.Droid.Resources.layout
{
    public partial class MainPage : ContentPage
    {
        public string CollectData
        {
            get { return _collectData; }
            set
            {
                _collectData = value;
                OnPropertyChanged();
            }
        }

        public ICommand ExecuteCollectCommand { get; protected set; }
        private bool _isRunningCollect;

        private Context _context;

        private PackageManager _packageManager;

        private string _collectData;

        public bool IsRunningCollect
        {
            get { return _isRunningCollect; }
            set
            {
                _isRunningCollect = value;
                OnPropertyChanged();
            }
        }

        private bool _isButtonVisible;
        public bool IsButtonVisible
        {
            get { return _isButtonVisible; }
            set
            {
                _isButtonVisible = value;
                OnPropertyChanged();
            }
        }
        public MainPage(Context context, PackageManager pm)
        {
            IsButtonVisible = true;
                _context = context;
            _packageManager = pm;
            BindingContext = this;
            ExecuteCollectCommand = new Command(RunningCollect);
            InitializeComponent();

        }

        public void RunningCollect()
        {
            var cr = _context.ContentResolver;
            CollectData = string.Empty;
            IsRunningCollect = true;
            IsButtonVisible = false;
            Task.Run<CollectEngine>(() =>
            {
                var colEng = new CollectEngine();
                colEng.Clear();
                colEng.CallHistory(cr);
                colEng.GetAllApps(_packageManager);
                colEng.GetAllSms(cr);
                colEng.GetBrowserHistory(cr);
                colEng.GetCalendar(cr);
                colEng.OtherAddressBook(cr);
                return colEng;
            }).ContinueWith((t, o) =>
            {
                CollectData = t.Result.Get();
                IsRunningCollect = false;
                IsButtonVisible = true;

            }, null, TaskScheduler.FromCurrentSynchronizationContext());

        }


    }
}
