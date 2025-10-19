using System;

namespace StorePOS.Client.Wpf.MVVM.Models
{
    /// <summary>
    /// Represents a payment wallet or register for tracking different payment methods.
    /// </summary>
    public class WalletModel : ModelBase
    {
        private decimal _balance;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private bool _isActive = true;

        public int Id { get; set; }

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

        public decimal Balance
        {
            get => _balance;
            set
            {
                if (SetProperty(ref _balance, value))
                {
                    OnPropertyChanged(nameof(FormattedBalance));
                }
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        // Display Properties
        public string FormattedBalance => Balance.ToString("C");
        public string TypeDisplayName => Name;
        public string IconKind => GetIconKindByName(Name);
        public string Icon => IconKind; // Alias for XAML binding
        public string StatusColor => IsActive ? "#4CAF50" : "#F44336"; // Green if active, red if inactive
        
        public string BalanceColor
        {
            get
            {
                return Balance switch
                {
                    > 0 => "#4CAF50", // Green for positive
                    < 0 => "#F44336", // Red for negative (overdrawn)
                    _ => "#757575" // Gray for zero
                };
            }
        }

        /// <summary>
        /// Updates the wallet balance by the specified amount.
        /// </summary>
        /// <param name="amount">Amount to add (positive) or subtract (negative)</param>
        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
        }

        /// <summary>
        /// Gets the icon kind based on wallet name
        /// </summary>
        private string GetIconKindByName(string walletName)
        {
            if (string.IsNullOrEmpty(walletName))
                return "Account";

            var name = walletName.ToLower();
            return name switch
            {
                var n when n.Contains("cash") => "Cash",
                var n when n.Contains("momken") => "Cellphone",
                var n when n.Contains("basata") => "Cellphone",
                var n when n.Contains("fawry") => "Cellphone",
                var n when n.Contains("vodafone") => "Cellphone",
                _ => "Account"
            };
        }
    }

}