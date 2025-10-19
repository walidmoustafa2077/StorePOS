namespace StorePOS.Client.Wpf.Enums
{
    /// <summary>
    /// Types of transactions that can affect the cash register.
    /// </summary>
    public enum TransactionType
    {
        Sale,
        Refund,
        CashDrop,
        CashAdd,
        PayIn,
        PayOut
    }
}