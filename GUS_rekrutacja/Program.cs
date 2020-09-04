using GUS_rekrutacja.Services;
using System;

namespace GUS_rekrutacja
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiGuiService = new ApiGusService();

            apiGuiService.CallWebService();

            Console.Read();
        }
    }
}