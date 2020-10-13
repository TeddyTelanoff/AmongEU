using System;

namespace DeticatedServer.Game
{
    class TaskSystem
    {
        public static readonly Task[] AllTasks = { };

        public readonly TaskType type;
        public Task[] tasks;

        public TaskSystem(uint numTasks, TaskType type)
        {
            this.type = type;

            Random rng = new Random();

            tasks = new Task[numTasks];
            for (int i = 0; i < numTasks; i++)
            {
                int num = rng.Next(AllTasks.Length);
                if (AllTasks[num].type == type)
                    tasks[i] = (Task)AllTasks[num].Clone();
            }
        }
    }

    enum TaskType
    {
        Short,
        Medium,
        Long
    }

    enum TaskID
    {
        Wires
    }

    abstract class Task : ICloneable
    {
        public TaskType type;
        public TaskID id;
        public bool completed;

        public abstract bool IsComplete();
        public bool TryComplete() => completed = IsComplete();

        public abstract object Clone();
    }
}
