using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCore.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageHandlerStats
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the total messages processed.
        /// </summary>
        /// <value>
        /// The total messages processed.
        /// </value>
        int TotalMessagesProcessed { get; }
        /// <summary>
        /// Gets the total messages failed.
        /// </summary>
        /// <value>
        /// The total messages failed.
        /// </value>
        int TotalMessagesFailed { get; }
        /// <summary>
        /// Gets the total retries.
        /// </summary>
        /// <value>
        /// The total retries.
        /// </value>
        int TotalRetries { get; }
        /// <summary>
        /// Gets the total normal messages received.
        /// </summary>
        /// <value>
        /// The total normal messages received.
        /// </value>
        int TotalNormalMessagesReceived { get; }
        /// <summary>
        /// Gets the total priority messages received.
        /// </summary>
        /// <value>
        /// The total priority messages received.
        /// </value>
        int TotalPriorityMessagesReceived { get; }
        /// <summary>
        /// Gets the last message processed.
        /// </summary>
        /// <value>
        /// The last message processed.
        /// </value>
        DateTime? LastMessageProcessed { get; }
        /// <summary>
        /// Adds the specified stats.
        /// </summary>
        /// <param name="stats">The stats.</param>
        void Add(IMessageHandlerStats stats);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.Messaging.IMessageHandlerStats" />
    public class MessageHandlerStats : IMessageHandlerStats
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerStats"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public MessageHandlerStats(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandlerStats"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="totalMessagesProcessed">The total messages processed.</param>
        /// <param name="totalMessagesFailed">The total messages failed.</param>
        /// <param name="totalRetries">The total retries.</param>
        /// <param name="totalNormalMessagesReceived">The total normal messages received.</param>
        /// <param name="totalPriorityMessagesReceived">The total priority messages received.</param>
        /// <param name="lastMessageProcessed">The last message processed.</param>
        public MessageHandlerStats(string name, int totalMessagesProcessed, int totalMessagesFailed, int totalRetries,
            int totalNormalMessagesReceived, int totalPriorityMessagesReceived, DateTime? lastMessageProcessed)
        {
            Name = name;
            TotalMessagesProcessed = totalMessagesProcessed;
            TotalMessagesFailed = totalMessagesFailed;
            TotalRetries = totalRetries;
            TotalNormalMessagesReceived = totalNormalMessagesReceived;
            TotalPriorityMessagesReceived = totalPriorityMessagesReceived;
            LastMessageProcessed = lastMessageProcessed;
        }
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the last message processed.
        /// </summary>
        /// <value>
        /// The last message processed.
        /// </value>
        public DateTime? LastMessageProcessed { get; private set; }
        /// <summary>
        /// Gets the total messages processed.
        /// </summary>
        /// <value>
        /// The total messages processed.
        /// </value>
        public int TotalMessagesProcessed { get; private set; }
        /// <summary>
        /// Gets the total messages failed.
        /// </summary>
        /// <value>
        /// The total messages failed.
        /// </value>
        public int TotalMessagesFailed { get; private set; }
        /// <summary>
        /// Gets the total retries.
        /// </summary>
        /// <value>
        /// The total retries.
        /// </value>
        public int TotalRetries { get; private set; }
        /// <summary>
        /// Gets the total normal messages received.
        /// </summary>
        /// <value>
        /// The total normal messages received.
        /// </value>
        public int TotalNormalMessagesReceived { get; private set; }
        /// <summary>
        /// Gets the total priority messages received.
        /// </summary>
        /// <value>
        /// The total priority messages received.
        /// </value>
        public int TotalPriorityMessagesReceived { get; private set; }

        /// <summary>
        /// Adds the specified stats.
        /// </summary>
        /// <param name="stats">The stats.</param>
        public virtual void Add(IMessageHandlerStats stats)
        {
            TotalMessagesProcessed += stats.TotalMessagesProcessed;
            TotalMessagesFailed += stats.TotalMessagesFailed;
            TotalRetries += stats.TotalRetries;
            TotalNormalMessagesReceived += stats.TotalNormalMessagesReceived;
            TotalPriorityMessagesReceived += stats.TotalPriorityMessagesReceived;
            if (LastMessageProcessed == null || stats.LastMessageProcessed > LastMessageProcessed)
                LastMessageProcessed = stats.LastMessageProcessed;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"STATS for {Name}:").AppendLine();            
            sb.AppendLine($"  TotalNormalMessagesReceived:    {TotalNormalMessagesReceived}");
            sb.AppendLine($"  TotalPriorityMessagesReceived:  {TotalPriorityMessagesReceived}");
            sb.AppendLine($"  TotalProcessed:                 {TotalMessagesProcessed}");
            sb.AppendLine($"  TotalRetries:                   {TotalRetries}");
            sb.AppendLine($"  TotalFailed:                    {TotalMessagesFailed}");
            sb.AppendLine($"  LastMessageProcessed:           {LastMessageProcessed?.ToString() ?? ""}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MessageHandlerStatsExtensions
    {
        /// <summary>
        /// Combines the stats.
        /// </summary>
        /// <param name="stats">The stats.</param>
        /// <returns></returns>
        public static IMessageHandlerStats CombineStats(this IEnumerable<IMessageHandlerStats> stats)
        {
            IMessageHandlerStats to = null;

            if (stats != null)
            {
                foreach (var stat in stats)
                {
                    if (to == null)
                        to = new MessageHandlerStats(stat.Name);

                    to.Add(stat);
                }
            }

            return to;
        }
    }
}