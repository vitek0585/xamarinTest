using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinForm
{
    public partial class MainPage : ContentPage
    {
        public ICommand ExecuteCollectCommand { get; protected set; }
        private bool _isRunningCollect;
        public bool IsRunningCollect
        {
            get { return _isRunningCollect; }
            set
            {
                _isRunningCollect = value;
                OnPropertyChanged();
            } 
        }

        public MainPage()
        {
            BindingContext = this;
            ExecuteCollectCommand = new Command(RunningCollect);
            InitializeComponent();

        }

        public void RunningCollect()
        {
            IsRunningCollect = !IsRunningCollect;
        }
       

    }
}
