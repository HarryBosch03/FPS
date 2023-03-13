namespace Code.Scripts.Player.Inventory
{
    public class ItemStack
    {
        public ItemType Type { get; }
        public int Amount { get; set; }

        public ItemStack(ItemType type, int amount = 0)
        {
            Type = type;
            Amount = amount;
        }

        public bool TryCombine(ItemStack other)
        {
            if (Type != other.Type) return false;

            var total = Amount + other.Amount;
            if (total > Type.maxStackSize)
            {
                Amount = Type.maxStackSize;
                other.Amount = total - Amount;
                return false;
            }

            Amount = total;
            other.Amount = 0;
            return true;
        }

        public static implicit operator bool(ItemStack stack)
        {
            if (stack == null) return false;
            if (stack.Type == null) return false;
            if (stack.Amount <= 0) return false;

            return true;
        }
    }
}
