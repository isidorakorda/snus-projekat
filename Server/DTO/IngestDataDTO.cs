namespace Server.DTO
{
    public record IngestDataDTO
    (
        Guid SensorId,
        double Temperature,
        int MessageId,
        DateTime Timestamp,
        int AlarmPriority
    );
}
