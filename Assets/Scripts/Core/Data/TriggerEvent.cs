using System.Collections.Generic;

public class TriggerEvent
{
    public string EventType { get; set; }
    public Character Source { get; set; }
    public Character Target { get; set; }
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}