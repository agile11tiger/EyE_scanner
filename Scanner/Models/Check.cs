using Newtonsoft.Json;
using Scanner.Models.Iterfaces;
using SQLite;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VerifyReceiptSDK.Results;

namespace Scanner.Models
{
    [DataContract]
    public class Check : ISerializableDatabaseItem, IDatabaseItem
    {
        public Check(Receipt receipt)
        {
            Receipt = receipt;
        }

        public Check()
        {
        }

        [PrimaryKey, AutoIncrement, DataMember]
        public int Id { get; set; }
        [DataMember]
        public CheckTypes Type { get; set; }
        [DataMember]
        public int FriendId { get; set; } = -1;
        [Ignore]
        public Receipt Receipt { get; private set; }
        [DataMember]
        public string ReceiptJson { get; set; }

        #region ISerializable
        public void Serialize()
        {
            ReceiptJson = JsonConvert.SerializeObject(Receipt, ISerializableDatabaseItem.JsonSettings);
        }

        public void Deserialize()
        {
            Receipt = JsonConvert.DeserializeObject<Receipt>(ReceiptJson, ISerializableDatabaseItem.JsonSettings);
        }
        #endregion

        #region IClonable
        public virtual Check PartialClone()
        {
            var check = (Check)MemberwiseClone();
            check.Receipt = Receipt.PartialClone();
            check.Receipt.Items = new List<Item>();
            return check;
        }
        #endregion
    }
}
