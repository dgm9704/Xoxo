namespace Diwen.Xbrl.Csv
{
    /// <summary/>
    public enum AllowedDuplicates
    {
        /// <summary>
        /// duplicate facts are not permitted
        /// </summary>
        None = 3,

        /// <summary>
        /// only complete duplicates are permitted
        /// </summary>
        Complete = 2,

        /// <summary>
        /// only consistent duplicates are permitted (note that the definition of consistent duplicates includes complete duplicates
        /// </summary>
        Consistent = 1,

        /// <summary>
        /// (default) all classes of duplicate facts are permitted
        /// </summary>
        All = 0,
    }
}