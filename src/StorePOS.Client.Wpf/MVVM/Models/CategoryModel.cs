namespace StorePOS.Client.Wpf.MVVM.Models
{
    public class CategoryModel : ModelBase
    {
        private int _id;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _icon = string.Empty;
        private string _color = string.Empty;
        private bool _isActive;
        private int _productCount;

        private bool _isSelected;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public string Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public int ProductCount
        {
            get => _productCount;
            set => SetProperty(ref _productCount, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public string DisplayColor => string.IsNullOrEmpty(Color) ? "#2196F3" : Color;
    }
}