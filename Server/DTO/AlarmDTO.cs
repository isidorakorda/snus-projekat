namespace Server.DTO
{
    public class AlarmDTO
    {
        public string Message;
        public int Priority;

        public AlarmDTO(string message, int priority)
        {
            Message = message;
            Priority = priority;
        }
    }
}
