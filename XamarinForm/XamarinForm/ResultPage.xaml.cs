using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamarinForm
{
    public partial class ResultPage : ContentPage
    {
        public FormattedString CollectData
        {
            get { return _collectData; }
            set
            {
                _collectData = value;
                OnPropertyChanged();
            }
        }


        private FormattedString _collectData;
        public ResultPage()
        {
            BindingContext = this;
            InitializeComponent();
        }
    }
}
