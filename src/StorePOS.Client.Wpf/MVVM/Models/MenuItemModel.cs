using System.ComponentModel;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    public class MenuItemModel : ModelBase
    {
        private string _name = string.Empty;
        private string _icon = string.Empty;
        private bool _isSelected;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public MenuItemModel(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}