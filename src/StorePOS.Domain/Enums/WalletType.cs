namespace StorePOS.Domain.Enums
{
    /// <summary>
    /// Types of wallets or payment methods available in the system.
    /// </summary>
    public enum WalletType
    {
        Cash = 0,
        CreditCard = 1,
        DebitCard = 2,
        DigitalWallet = 3,
        GiftCard = 4,
        Other = 5
    }
}