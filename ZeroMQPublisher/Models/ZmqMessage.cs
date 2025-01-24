namespace ZeroMQPublisher.Models
{
    public class ZmqMessage(string topic, byte[] message)
    {
        #region Properties

        public string Topic
        {
            get;
            private set;
        } = topic;

        public byte[] Message
        {
            get;
            private set;
        } = message;

        #endregion Properties
    }
}