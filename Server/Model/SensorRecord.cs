namespace Server.Model
{
    public class SensorRecord
    {
        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public double Temperature { get; set; }
        public int MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public int AlarmPriority { get; set; }
    }
}