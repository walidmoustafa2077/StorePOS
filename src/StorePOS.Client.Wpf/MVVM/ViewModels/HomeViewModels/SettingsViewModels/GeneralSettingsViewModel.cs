using System.Collections.ObjectModel;

namespace StorePOS.Client.Wpf.MVVM.ViewModels.HomeViewModels.Settings
{
    /// <summary>
    /// ViewModel for General Settings
    /// </summary>
    public class GeneralSettingsViewModel : ViewModelBase
    {
        private string _storeName = "My Store";
        private string _storeAddress = "";
        private string _contactPhone = "";
        private string _contactEmail = "";
        private string? _selectedBusinessType;
        private string? _selectedCurrency;
        private string? _selectedTimeZone;
        private string? _selectedDateFormat;
        private string? _selectedTimeFormat;
        private string? _selectedThemeMode;
        private string? _selectedPrimaryColor;
        private string? _selectedFontSize;
        private TimeSpan? _openingTime = new TimeSpan(9, 0, 0);
        private TimeSpan? _closingTime = new TimeSpan(18, 0, 0);

        public string StoreName
        {
            get => _storeName;
            set => SetProperty(ref _storeName, value);
        }

        public string StoreAddress
        {
            get => _storeAddress;
            set => SetProperty(ref _storeAddress, value);
        }

        public string ContactPhone
        {
            get => _contactPhone;
            set => SetProperty(ref _contactPhone, value);
        }

        public string ContactEmail
        {
            get => _contactEmail;
            set => SetProperty(ref _contactEmail, value);
        }

        public string? SelectedBusinessType
        {
            get => _selectedBusinessType;
            set => SetProperty(ref _selectedBusinessType, value);
        }

        public string? SelectedCurrency
        {
            get => _selectedCurrency;
            set => SetProperty(ref _selectedCurrency, value);
        }

        public string? SelectedTimeZone
        {
            get => _selectedTimeZone;
            set => SetProperty(ref _selectedTimeZone, value);
        }

        public string? SelectedDateFormat
        {
            get => _selectedDateFormat;
            set => SetProperty(ref _selectedDateFormat, value);
        }

        public string? SelectedTimeFormat
        {
            get => _selectedTimeFormat;
            set => SetProperty(ref _selectedTimeFormat, value);
        }

        public string? SelectedThemeMode
        {
            get => _selectedThemeMode;
            set => SetProperty(ref _selectedThemeMode, value);
        }

        public string? SelectedPrimaryColor
        {
            get => _selectedPrimaryColor;
            set => SetProperty(ref _selectedPrimaryColor, value);
        }

        public string? SelectedFontSize
        {
            get => _selectedFontSize;
            set => SetProperty(ref _selectedFontSize, value);
        }

        public TimeSpan? OpeningTime
        {
            get => _openingTime;
            set => SetProperty(ref _openingTime, value);
        }

        public TimeSpan? ClosingTime
        {
            get => _closingTime;
            set => SetProperty(ref _closingTime, value);
        }

        public ObservableCollection<string> BusinessTypes { get; }
        public ObservableCollection<string> Currencies { get; }
        public ObservableCollection<string> TimeZones { get; }
        public ObservableCollection<string> DateFormats { get; }
        public ObservableCollection<string> TimeFormats { get; }
        public ObservableCollection<string> ThemeModes { get; }
        public ObservableCollection<string> PrimaryColors { get; }
        public ObservableCollection<string> FontSizes { get; }

        public GeneralSettingsViewModel()
        {
            BusinessTypes = new ObservableCollection<string>
            {
                "Retail", "Restaurant", "Grocery", "Convenience Store", "Other"
            };

            Currencies = new ObservableCollection<string>
            {
                "USD - US Dollar", "EUR - Euro", "GBP - British Pound", "EGP - Egyptian Pound"
            };

            TimeZones = new ObservableCollection<string>
            {
                "(UTC-05:00) Eastern Time", "(UTC) Greenwich Mean Time", "(UTC+02:00) Cairo"
            };

            DateFormats = new ObservableCollection<string>
            {
                "MM/DD/YYYY", "DD/MM/YYYY", "YYYY-MM-DD"
            };

            TimeFormats = new ObservableCollection<string>
            {
                "12 Hour (AM/PM)", "24 Hour"
            };

            ThemeModes = new ObservableCollection<string>
            {
                "Light", "Dark", "System Default"
            };

            PrimaryColors = new ObservableCollection<string>
            {
                "Blue", "Green", "Purple", "Orange", "Red"
            };

            FontSizes = new ObservableCollection<string>
            {
                "Small", "Medium", "Large"
            };

            // Set defaults
            SelectedBusinessType = BusinessTypes.FirstOrDefault();
            SelectedCurrency = Currencies.FirstOrDefault();
            SelectedTimeZone = TimeZones.FirstOrDefault();
            SelectedDateFormat = DateFormats.FirstOrDefault();
            SelectedTimeFormat = TimeFormats.FirstOrDefault();
            SelectedThemeMode = ThemeModes.FirstOrDefault();
            SelectedPrimaryColor = PrimaryColors.FirstOrDefault();
            SelectedFontSize = FontSizes[1]; // Medium
        }
    }
}
