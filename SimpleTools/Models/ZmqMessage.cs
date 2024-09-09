namespace SimpleTools.Models
{
    public class ZmqMessage
    {
        #region Constructor

        public ZmqMessage(string topic, byte[] message)
        {
            Topic = topic;
            Message = message;
        }

        #endregion Constructor

        #region Properties

        public string Topic
        {
            get;
            private set;
        }

        public byte[] Message
        {
            get;
            private set;
        }

        #endregion Properties
    }
}