using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.IntegrationServices;
 
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SSIS_Status
{
   

    public class ssis
    {

        public void get()
        {

            const string serverName = "PWNC0429SSQL01";
            const string catalogName = "SSISDB";

            var ssisConnectionString = $"Data Source={serverName};Initial Catalog=msdb;Integrated Security=SSPI;";
            var ids = new List<long> { 3105 };

            var idCount = ids.Count();
            var previousCount = -1;

            var iterations = 0;

            try
            {
               

                const int secondsToSleep = 1;
                var sleepTime = TimeSpan.FromSeconds(secondsToSleep);
                var maxIterations = TimeSpan.FromHours(1).TotalSeconds / sleepTime.TotalSeconds;
                
                IDictionary<long, Operation.ServerOperationStatus> catalogExecutions;
                using (var connection = new SqlConnection(ssisConnectionString))
                {
                    var server = new IntegrationServices(connection);
                    var catalog = server.Catalogs[catalogName];
                    do
                    {
                        catalogExecutions = catalog.Executions
                            .Where(execution => ids.Contains(execution.Id))
                            .ToDictionary(execution => execution.Id, execution => execution.Status);

                        var runningCount = catalogExecutions.Count(kvp => kvp.Value == Operation.ServerOperationStatus.Running);
                        System.Threading.Thread.Sleep(sleepTime);

                        //Dts.Events.FireInformation(0, "ScriptMain", $"{runningCount} packages still running.", string.Empty, 0, ref fireAgain);

                        if (runningCount != previousCount)
                        {
                            previousCount = runningCount;
                            decimal completed = idCount - runningCount;
                            decimal percentCompleted = completed / idCount;
                            Console.WriteLine($"Waiting... {completed}/{idCount} completed", Convert.ToInt32(100 * percentCompleted), 0, 0, "");
                        }

                        iterations++;
                        if (iterations >= maxIterations)
                        {
                            
                            return;
                        }
                    }
                    while (catalogExecutions.Any(kvp => kvp.Value == Operation.ServerOperationStatus.Running));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Error");
            }

            
        }
    }
}
