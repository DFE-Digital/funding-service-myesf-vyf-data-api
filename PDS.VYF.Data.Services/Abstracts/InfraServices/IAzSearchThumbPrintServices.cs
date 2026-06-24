namespace PDS.VYF.Data.Services.Abstracts.InfraServices
{
    /// <summary>
    /// The Azure search thumb print services.
    /// </summary>
    public interface IAzSearchThumbPrintServices
    {
        /// <summary>
        /// Azs the index thumb print.
        /// </summary>
        /// <param name="keyParams">The key parameters.</param>
        /// <param name="types">The types.</param>
        /// <returns>The Thumb print.</returns>
        int AzIndexThumbPrint(List<string> keyParams, params Type[] types);
    }
}
