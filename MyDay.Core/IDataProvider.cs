using System.Threading.Tasks;

namespace MyDay.Core
{
    public interface IDataProvider
    {
        /// <summary>
        /// The command name that is passed on the
        /// command line to run this data provider
        /// </summary>
        string Command { get; }

        /// <summary>
        /// The name of this data provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the data provider
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Is this data provider enabled?
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Is this data provider configured
        /// </summary>
        bool Configured { get; }

        /// <summary>
        /// Configure or reconfigure this data provider
        /// </summary>
        void Configure();

        /// <summary>
        /// Execute this data provider
        /// </summary>
        /// <param name="full">If true, displays full/long information</param>
        Task Execute(bool full);
    }
}
