using System.Collections.Concurrent;

namespace ChessLiteServerAPI
{
    public class TaskManager
    {
        // Use a thread-safe collection to store tasks
        private readonly ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();

        // Add a new task to the manager
        public void AddTask(Task task)
        {
            _tasks.Add(task);
        }

        // Wait for all tasks to complete
        public async Task WaitForAllTasks()
        {
            try
            {
                await Task.WhenAll(_tasks); // Await all tasks
                Console.WriteLine("All background tasks completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during task completion: {ex.Message}");
            }
        }
    }
}
