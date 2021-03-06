﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using ProviderCancellationRule.Entities;

namespace ProviderCancellationRule
{
    class CancellationRulePriorityUpdater
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;

        public CancellationRulePriorityUpdater(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public void Execute()
        {
            List<Provider> providers = GetAllProviders();
            foreach (var provider in providers.Where(provider => provider.Id == 2180 ) ) // TODO: убрать перед использованием
            {
                    ProviderCancellationRulePriorityUpdater providerCancellationRulePriorityUpdater =
                        new ProviderCancellationRulePriorityUpdater(provider, _connectionString, _logger);
                    providerCancellationRulePriorityUpdater.Execute();
            }
        }

        List<Provider> GetAllProviders()
        {
            List<Provider> result = new List<Provider>();
            string queryString = "select p.*, c.tzid  from [provider] p inner join city c on p.id_city = c.id_city";
            using ( SqlConnection connection = new SqlConnection( _connectionString ) )
            {
                SqlCommand command = new SqlCommand( queryString, connection );
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while ( reader.Read() )
                    {
                        result.Add( new Provider( Convert.ToString( reader[ "tzid" ] ) )
                        {
                            Id = Convert.ToInt32( reader[ "id_provider" ] ),
                            Name = Convert.ToString( reader[ "name" ] ),
                            ArrivalTime = ( reader[ "arrival_time" ] is DBNull )
                                ? Time.Null
                                : ( Time )Convert.ToInt32( reader[ "arrival_time" ] ),
                            DepartureTime = ( reader[ "departure_time" ] is DBNull )
                            ? Time.Null
                            : ( Time )Convert.ToInt32( reader[ "departure_time" ] ),
                        });
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return result;
        }
    }
}
