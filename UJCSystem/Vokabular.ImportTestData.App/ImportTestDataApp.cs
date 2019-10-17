using System;
using Vokabular.AppAuthentication.Shared;

namespace Vokabular.ImportTestData.App
{
    public class ImportTestDataApp
    {
        private readonly DataProvider m_dataProvider;
        private readonly AuthenticationManager m_authenticationManager;
        private readonly AuthenticationOptions m_authOptions;
        private readonly ImportTestProjectManager m_importTestProjectManager;
        public const string Separator = "------------------------------------------------------------------------------";

        public ImportTestDataApp(DataProvider dataProvider, AuthenticationManager authenticationManager, AuthenticationOptions authOptions,
            ImportTestProjectManager importTestProjectManager)
        {
            m_dataProvider = dataProvider;
            m_authenticationManager = authenticationManager;
            m_authOptions = authOptions;
            m_importTestProjectManager = importTestProjectManager;
        }

        public void Run()
        {
            var output = m_dataProvider.Output;

            output.WriteLine(Separator);
            output.WriteLine("Import test data to Vokabulář app");
            output.WriteLine("> The main purpose of this app is testing performance of whole solution when it contains large amount of projects");
            output.WriteLine(Separator);
            output.WriteLine();

            output.WriteLine("Authentication is required (browser with login page should be opened)");
            m_authenticationManager.SignInAsync(m_authOptions).GetAwaiter().GetResult();

            output.WriteLine(Separator);
            output.WriteLine();

            output.WriteLine("Generated projects will have number suffix in their names. Please, specify first and last number for generated sequence.");
            var firstNumber = m_dataProvider.GetNumber("First number:");
            var lastNumber = m_dataProvider.GetNumber("Last number:");
            output.WriteLine(Separator);
            output.WriteLine();

            if (firstNumber > lastNumber)
            {
                output.WriteLine("Invalid input values. The first number must be lower than the last number.");
                output.WriteLine("Import cancelled");
                m_dataProvider.GetString("Press any key to close ...");
                return;
            }

            for (int i = firstNumber; i <= lastNumber; i++)
            {
                output.WriteLine($"Importing testing project {i}");
                m_importTestProjectManager.Import(i);
            }
            output.WriteLine(Separator);
            output.WriteLine();

            output.WriteLine("Import finished");
            m_dataProvider.GetString("Press any key to close ...");
        }
    }
}