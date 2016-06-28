using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Content;
using Android.Content.PM;
using Xamarin.Forms;

namespace XamarinForm.Droid.Resources.layout
{
    public partial class MainPage : ContentPage
    {
        public ICommand ExecuteCollectCommand { get; protected set; }
        private bool _isRunningCollect;

        private Context _context;

        private PackageManager _packageManager;

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
            Padding = Device.OnPlatform(new Thickness(0, 20, 0, 0), new Thickness(0, 20, 0, 0), new Thickness(0));
            InitializeComponent();

        }

        public async void RunningCollect()
        {
            var cr = _context.ContentResolver;
            IsRunningCollect = true;
            IsButtonVisible = false;
            var result = await Task.Run<CollectEngine>(() =>
            {
                var colEng = new CollectEngine();

                colEng.CollectCalendarEvents(cr);
                colEng.CallHistory(cr);
                colEng.CollectApplications(_packageManager);
                colEng.GetAllSms(cr);
                colEng.GetBrowserHistory(cr);
                colEng.OtherAddressBook(cr);
                return colEng;

            });
            IsRunningCollect = false;
            IsButtonVisible = true;
            HttpClient client = new HttpClient();
            await Navigation.PushAsync(new ResultPage()
            {
                Content = new ScrollView() { Content = result.Get() },
                Padding = Device.OnPlatform<Thickness>(0, new Thickness(20), 0)
            });
            //await client.PostAsync("http://127.0.0.1:49544/Home/DataCollect", new StringContent(result.GetString(),
            //Encoding.UTF8,
            //"application/json"));
        }
    }
}
