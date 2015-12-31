using DataImportConsole.NewHomeProcess;
using System;
namespace DataImportConsole
{
    public class NewHomeFeedProcessor
    {
        void Main(string[] args) //public static 
        {
            try
            {
                NewHomeBaseFeedProcess.RunProcess();

            }
            catch (Exception ex) { };
            try
            {
                BDXFeedProcess.RunProcess();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
