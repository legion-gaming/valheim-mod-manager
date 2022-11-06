using Prism.Events;

namespace ValheimModManager.Core.Data;

public class TaskStatusEvent : PubSubEvent<TaskStatusEvent>
{
    public bool IsCompleted { get; set; }
}