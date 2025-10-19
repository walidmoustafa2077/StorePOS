using StorePOS.Client.Wpf.Enums;

namespace StorePOS.Client.Wpf.Extensions
{
    /// <summary>
    /// Extension methods for CashFlowType enum.
    /// Provides display names and icon mappings for cash flow types.
    /// </summary>
    public static class CashFlowTypeExtensions
    {
        /// <summary>
        /// Gets the display name for a cash flow type.
        /// </summary>
        /// <param name="type">The cash flow type</param>
        /// <returns>A user-friendly display name</returns>
        public static string GetDisplayName(this CashFlowType type)
        {
            return type switch
            {
                CashFlowType.Sale => "Sale",
                CashFlowType.Refund => "Refund",
                CashFlowType.CashDrop => "Cash Drop",
                CashFlowType.CashAdd => "Cash Add",
                CashFlowType.StartingCash => "Starting Cash",
                CashFlowType.EndingCash => "Ending Cash",
                CashFlowType.Other => "Other",
                _ => type.ToString()
            };
        }

        /// <summary>
        /// Gets the icon kind for a cash flow type.
        /// Uses Material Design Icons naming convention.
        /// </summary>
        /// <param name="type">The cash flow type</param>
        /// <returns>The icon name/kind for the cash flow type</returns>
        public static string GetIconKind(this CashFlowType type)
        {
            return type switch
            {
                CashFlowType.Sale => "CashRegister",
                CashFlowType.Refund => "CashRefund",
                CashFlowType.CashDrop => "TrayArrowDown",
                CashFlowType.CashAdd => "TrayArrowUp",
                CashFlowType.StartingCash => "CashPlus",
                CashFlowType.EndingCash => "CashMinus",
                CashFlowType.Other => "Cash",
                _ => "Cash"
            };
        }

        /// <summary>
        /// Gets the color associated with a cash flow type.
        /// </summary>
        /// <param name="type">The cash flow type</param>
        /// <returns>A hex color code for the cash flow type</returns>
        public static string GetTypeColor(this CashFlowType type)
        {
            return type switch
            {
                CashFlowType.Sale => "#4CAF50",        // Green
                CashFlowType.CashAdd => "#4CAF50",     // Green
                CashFlowType.StartingCash => "#2196F3", // Blue
                CashFlowType.Refund => "#F44336",      // Red
                CashFlowType.CashDrop => "#FF9800",    // Orange
                CashFlowType.EndingCash => "#9C27B0",  // Purple
                CashFlowType.Other => "#757575",       // Gray
                _ => "#757575"                         // Default Gray
            };
        }

        /// <summary>
        /// Determines if the cash flow type represents incoming money.
        /// </summary>
        /// <param name="type">The cash flow type</param>
        /// <returns>True if it's incoming money, false otherwise</returns>
        public static bool IsIncoming(this CashFlowType type)
        {
            return type switch
            {
                CashFlowType.Sale => true,
                CashFlowType.CashAdd => true,
                CashFlowType.StartingCash => true,
                _ => false
            };
        }

        /// <summary>
        /// Determines if the cash flow type represents outgoing money.
        /// </summary>
        /// <param name="type">The cash flow type</param>
        /// <returns>True if it's outgoing money, false otherwise</returns>
        public static bool IsOutgoing(this CashFlowType type)
        {
            return type switch
            {
                CashFlowType.Refund => true,
                CashFlowType.CashDrop => true,
                _ => false
            };
        }
    }
}