using System;

namespace Play.Inventory.Service.Exceptions
{
    [Serializable]
    public class UnknowItemException : Exception
    {
        public Guid ItemID { get; }

        public UnknowItemException(Guid itemID)
            : base($"Unknow item '{itemID}'")
        {
            ItemID = itemID;
        }
    }
}
