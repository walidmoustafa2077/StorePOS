using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Extensions
{
    public static class TransactionTypeExtensions
    {
        public static string GetDisplayName(this TransactionType type)
        {
            return type switch
            {
                TransactionType.Sale => "Sale",
                TransactionType.Refund => "Refund",
                TransactionType.CashDrop => "Cash Drop",
                TransactionType.CashAdd => "Cash Add",
                TransactionType.PayIn => "Pay In",
                TransactionType.PayOut => "Pay Out",
                _ => type.ToString()
            };
        }

        public static string GetIconKind(this TransactionType type)
        {
            return type switch
            {
                TransactionType.Sale => "CashRegister",
                TransactionType.Refund => "CashRefund",
                TransactionType.CashDrop => "CashMinus",
                TransactionType.CashAdd => "CashPlus",
                TransactionType.PayIn => "CashPlus",
                TransactionType.PayOut => "CashMinus",
                _ => "Cash"
            };
        }
    }
}