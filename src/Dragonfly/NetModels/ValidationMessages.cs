namespace Dragonfly.NetModels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    public interface IValidationMessages
    {
        bool IsSuccessConfirmation { get; set; }

        string FormName { get; set; }

        IList<string> Messages { get; }
    }

    [DataContract(IsReference = true)]
    [Serializable]
    public class ValidationMessages : IValidationMessages
    {
        private readonly List<string> _messages;

        [DataMember]
        public bool IsSuccessConfirmation { get; set; }

        public string FormName { get; set; }

        [DataMember]
        public IList<string> Messages
        {
            get
            {
                return (IList<string>)this._messages;
            }

            set
            {
                this._messages.AddRange(value);
            }
        }

        public ValidationMessages()
        {
            this._messages = new List<string>();
        }
    }
}

