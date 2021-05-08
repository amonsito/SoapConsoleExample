using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ConsoleApp1
{
    class Program
    {

        //Generar contrato con el comando de power shell

        //svcutil http://www.dneonline.com/calculator.asmx?wsdl /Language=c# /t:Code /out:ClassNameProxy.cs /config:ClassNameProxy.config

        static void Main(string[] args)
        {
            Forma1Proxy();
            Forma2Simple();
        }

        private static void Forma2Simple()
        {
            var url = "http://www.dneonline.com/calculator.asmx";
            var b = new BasicHttpBinding();
            var c = new EndpointAddress(url);
            var a = new ChannelFactory<CalculatorSoapChannel>(b, c);

            a.Open();
            var response = a.CreateChannel().Add(10, 5);
            a.Close();
        }

        private static void Forma1Proxy()
        {
            Console.WriteLine("Hello World!");
            var a = new GenericProxy<CalculatorSoapChannel>("http://www.dneonline.com/calculator.asmx", 60);

            var b = a.Execute(x => x.Subtract(15, 5));
        }
    }
}
