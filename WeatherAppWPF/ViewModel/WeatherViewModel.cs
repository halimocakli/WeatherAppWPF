using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WeatherAppWPF.Model;
using WeatherAppWPF.ViewModel.Commands;
using WeatherAppWPF.ViewModel.Helper;

namespace WeatherAppWPF.ViewModel
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        public WeatherViewModel()
        {
            /*
             * DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()) ifadesi,
             * Eğer program dizayn modundaysa, yani biz henüz tasarım aşamasında ya da programlama aşamasındaysak,
             * if bloğu içerisindeki ifadeler çalışacak ve belirttiğimiz konfigürasyonda görünecetir. Aksi halde,
             * yani programın çalışması esnasında if bloğu içerisindeki ifadeler çalışmayacaktır.
             */
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                SelectedCity = new City
                {
                    LocalizedName = "Istanbul"
                };

                CurrentConditions = new CurrentConditions
                {
                    WeatherText = "Partly Cloudy",

                    Temperature = new Temperature
                    {
                        Metric = new Units
                        {
                            Value = "21"
                        }
                    }
                };
            }

            SearchCommand = new SearchCommand(this);

            Cities = new ObservableCollection<City>();
        }

        private string _query;

        public string Query
        {
            get { return _query; }
            set
            {
                _query = value;
                // Set kısmında _query field'ının içerisindeki değeri değiştiriyoruz.
                // Bu değişim sonucunda OnPropertChanged metodunu tetikleyerek değişimi bildirmemiz gerekiyor.
                OnPropertyChanged("Query");
            }
        }

        private CurrentConditions _currentConditions;

        public CurrentConditions CurrentConditions
        {
            get { return _currentConditions; }
            set
            {
                _currentConditions = value;
                OnPropertyChanged("CurrentConditions");
            }
        }

        private City _selectedCity;

        public City SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                OnPropertyChanged("SelectedCity");
                GetCurrentConitions();
            }
        }

        public async void MakeQuery()
        {
            var cities = await AccuWeatherHelper.GetCities(Query);

            Cities.Clear();

            foreach (var city in cities)
            {
                Cities.Add(city);
            }
        }

        public SearchCommand SearchCommand { get; set; }

        public ObservableCollection<City> Cities { get; set; }

        private async void GetCurrentConitions()
        {
            Query = string.Empty;
            Cities.Clear();

            CurrentConditions = await AccuWeatherHelper.GetCurrrentConditions(SelectedCity.Key);
        }


        // Property içerisindeki değer değiştiğinde tetiklenen event.
        // Bu event tetiklendikten sonra kendisine bağlı tüm yapıların içeriğini değiştirir.
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
