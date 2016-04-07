namespace MyDevoxx.Services.RestModel
{
    public class Slot
    {
        public string roomId { get; set; }
        public bool notAllocated { get; set; }
        public long fromTimeMillis { get; set; }
        public Break @break { get; set; }
        public string roomSetup { get; set; }
        public Talk talk { get; set; }
        public string fromTime { get; set; }
        public long toTimeMillis { get; set; }
        public string toTime { get; set; }
        public int roomCapacity { get; set; }
        public string roomName { get; set; }
        public string slotId { get; set; }
        public string day { get; set; }
    }
}
